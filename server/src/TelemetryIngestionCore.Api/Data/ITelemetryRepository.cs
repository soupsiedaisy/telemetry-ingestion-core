using TelemetryIngestionCore.Api.Models;

namespace TelemetryIngestionCore.Api.Data
{
    public interface ITelemetryRepository
    {
        public abstract Task<TelemetryReading> CreateAsync(
            TelemetryReading reading,
            CancellationToken ct = default
        );

        public abstract Task<IReadOnlyList<TelemetryReading>> QueryAsync(
            string? tenantId = null,
            string? deviceId = null,
            string? type = null,
            DateTimeOffset? from = null,
            DateTimeOffset? to = null,
            int? page = null,
            int? pageSize = null,
            CancellationToken ct = default
        );
    }
}
