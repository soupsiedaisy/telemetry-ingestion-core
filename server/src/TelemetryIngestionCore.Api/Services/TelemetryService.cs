using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using TelemetryIngestionCore.Api.Configuration;
using TelemetryIngestionCore.Api.Data;
using TelemetryIngestionCore.Api.Models;

namespace TelemetryIngestionCore.Api.Services;

public class TelemetryService(ITelemetryRepository repository) : ITelemetryService
{
    private readonly int batteryLowThreshold;
    private readonly int maxPageSize;

    public TelemetryService(ITelemetryRepository repository, IOptions<AppOptions> options)
        : this(repository)
    {
        batteryLowThreshold = options.Value.BatteryLowThreshold;
        maxPageSize = options.Value.MaxPageSize;
    }

    public async Task<TelemetryView> CreateReadingAsync(
        TelemetryDto dto,
        CancellationToken ct = default
    )
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var validationContext = new ValidationContext(dto);

        Validator.ValidateObject(dto, validationContext);

        var reading = new TelemetryReading
        {
            Id = Guid.Empty,
            TenantId = dto.TenantId,
            ExternalId = dto.ExternalId ?? string.Empty,
            DeviceId = dto.DeviceId,
            Type = dto.Type,
            Unit = dto.Unit,
            Value = dto.Value,
            Battery = dto.Battery,
            Signal = dto.Signal,
            RecordedAt = dto.RecordedAt,
            CreatedAt = default,
        };

        var created = await repository.CreateAsync(reading, ct).ConfigureAwait(false);

        return MapToView(created);
    }

    public async Task<IReadOnlyList<TelemetryView>> QueryReadingsAsync(
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
        if (pageSize > maxPageSize)
        {
            throw new ArgumentException(
                $"Page size of {pageSize} is greater than configured max page size of {maxPageSize}"
            );
        }

        var readings = await repository
            .QueryAsync(tenantId, deviceId, type, from, to, page, pageSize, ct)
            .ConfigureAwait(false);

        return [.. readings.Select(MapToView)];
    }

    private TelemetryView MapToView(TelemetryReading r) =>
        new()
        {
            TenantId = r.TenantId,
            ExternalId = string.IsNullOrEmpty(r.ExternalId) ? null : r.ExternalId,
            DeviceId = r.DeviceId,
            Type = r.Type,
            Unit = r.Unit,
            Value = r.Value,
            Battery = r.Battery,
            BatteryLow = r.Battery <= batteryLowThreshold,
            Signal = r.Signal,
            RecordedAt = r.RecordedAt.ToString(),
            CreatedAt = r.CreatedAt.ToString(),
        };
}
