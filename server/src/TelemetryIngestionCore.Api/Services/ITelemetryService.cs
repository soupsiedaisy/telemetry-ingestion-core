using TelemetryIngestionCore.Api.Models;

namespace TelemetryIngestionCore.Api.Services;

public interface ITelemetryService
{
    public abstract Task<TelemetryView> CreateReadingAsync(
        TelemetryDto dto,
        CancellationToken ct = default
    );
    public abstract Task<IReadOnlyList<TelemetryView>> QueryReadingsAsync(
        string? tenantId = null,
        string? deviceId = null,
        string? type = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        int? page = 1,
        int? pageSize = 50,
        CancellationToken ct = default
    );
}
