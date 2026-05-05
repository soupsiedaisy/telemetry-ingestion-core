using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TelemetryIngestionCore.Api.Configuration;
using TelemetryIngestionCore.Api.Exceptions;
using TelemetryIngestionCore.Api.Models;

namespace TelemetryIngestionCore.Api.Data;

/// <param name="context">The TelemetryContext injected by the program.</param>
/// <param name="options">The IOptions app options injected by the program.</param>
/// <inheritdoc cref="ITelemetryRepository" />
public class TelemetryRepository(TelemetryContext context, IOptions<AppOptions> options)
    : ITelemetryRepository
{
    /// <exception cref="ArgumentNullException">The exception thrown when the method is passed a null TelemetryReading.</exception>
    /// <exception cref="DuplicateExternalIdException">The exception thrown when a TelemetryReading with same TenantId and ExternalId is found.</exception>
    /// <inheritdoc cref="ITelemetryRepository.CreateAsync(TelemetryReading, CancellationToken)" />
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

    /// <inheritdoc cref="ITelemetryRepository.QueryAsync(string?, string?, string?, DateTimeOffset?, DateTimeOffset?, int?, int?, CancellationToken)" />
    public async Task<TelemetryReadingsPaginationData> QueryAsync(
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

        var readings = await query
            .OrderByDescending(reading => reading.RecordedAt)
            .Skip(skip)
            .Take(pageSizeValue)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var count = await context.TelemetryReadings.CountAsync().ConfigureAwait(false);

        var pageCount = (int)Math.Ceiling((double)count / pageSizeValue);

        return new TelemetryReadingsPaginationData
        {
            TelemetryReadings = readings,
            PaginationMetadata = new PaginationMetadata
            {
                PageCount = pageCount,
                PageNumber = pageValue,
                PageSize = pageSizeValue,
            },
        };
    }
}
