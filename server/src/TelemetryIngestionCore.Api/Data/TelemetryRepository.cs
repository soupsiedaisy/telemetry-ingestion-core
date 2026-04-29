using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TelemetryIngestionCore.Api.Configuration;
using TelemetryIngestionCore.Api.Exceptions;
using TelemetryIngestionCore.Api.Models;

namespace TelemetryIngestionCore.Api.Data;

public class TelemetryRepository(TelemetryContext context, IOptions<AppOptions> options)
    : ITelemetryRepository
{
    public async Task<TelemetryReading> CreateAsync(
        TelemetryReading reading,
        CancellationToken ct = default
    )
    {
        if (reading == null)
            throw new ArgumentNullException(nameof(reading));

        var existing = await context
            .TelemetryReadings.AsNoTracking()
            .FirstOrDefaultAsync(
                r =>
                    r.TenantId == reading.TenantId
                    && !string.IsNullOrWhiteSpace(r.ExternalId)
                    && r.ExternalId == reading.ExternalId,
                ct
            )
            .ConfigureAwait(false);

        if (existing != default)
        {
            throw new DuplicateExternalIdException(
                reading.TenantId,
                reading.ExternalId,
                existing?.Id
            );
        }

        if (reading.Id == Guid.Empty)
            reading.Id = Guid.NewGuid();

        reading.CreatedAt =
            reading.CreatedAt == default ? DateTimeOffset.UtcNow : reading.CreatedAt;

        context.TelemetryReadings.Add(reading);

        await context.SaveChangesAsync(ct).ConfigureAwait(false);

        return reading;
    }

    public async Task<IReadOnlyList<TelemetryReading>> QueryAsync(
        string? tenantId = null,
        string? deviceId = null,
        string? type = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        int? page = null,
        int? pageSize = null,
        CancellationToken ct = default
    )
    {
        var query = context.TelemetryReadings.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(tenantId))
            query = query.Where(r => r.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(deviceId))
            query = query.Where(r => r.DeviceId == deviceId);

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(r => r.Type == type);

        if (from.HasValue)
            query = query.Where(r => DateTimeOffset.Compare(from.Value, r.RecordedAt) <= 0);

        if (to.HasValue)
            query = query.Where(r => DateTimeOffset.Compare(r.RecordedAt, to.Value) <= 0);

        var pageValue = (!page.HasValue || page.Value < 1) ? 1 : page.Value;

        var pageSizeValue =
            (!pageSize.HasValue || pageSize.Value < 1)
                ? options.Value.DefaultPageSize
                : pageSize.Value;

        var skip = (pageValue - 1) * pageSizeValue;

        return await query
            .OrderByDescending(reading => reading.RecordedAt)
            .Skip(skip)
            .Take(pageSizeValue)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }
}
