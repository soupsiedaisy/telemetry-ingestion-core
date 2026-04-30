using TelemetryIngestionCore.Api.Models;

namespace TelemetryIngestionCore.Api.Services;

/// <summary>
/// A service responsible for processing transactions between the API layer and database layer.
/// Validates data transfer objects, applies domain rules and transforms between view, db and dto models.
/// </summary>
public interface ITelemetryService
{
    /// <summary>
    /// Creates a TelemetryReading entry in the database via the TelemetryRepository.
    /// </summary>
    /// <param name="dto">The TelemetryDto to transform and pass to the repository for database creation. Required.</param>
    /// <param name="ct">The CancellationToken for the operation.</param>
    /// <returns>An async operation with the created TelemetryReading as a view model.</returns>
    public abstract Task<TelemetryView> CreateReadingAsync(
        TelemetryDto dto,
        CancellationToken ct = default
    );

    /// <summary>
    /// Queries the TelemetryReading entries in the database via the TelemetryRepository.
    /// </summary>
    /// <param name="tenantId">The TelemetryReading tenantId to filter the responses with. Optional.</param>
    /// <param name="deviceId">The TelemetryReading deviceId to filter the responses with. Optional.</param>
    /// <param name="type">The TelemetryReading type to filter the responses with. Optional.</param>
    /// <param name="from">A 'from' date to filter the TelemetryReading responses with, based on the recordedAt property. Optional.</param>
    /// <param name="to">A 'to' date to filter the TelemetryReading responses with, based on the recordedAt property. Optional.</param>
    /// <param name="page">The page number for pagination. Optional.</param>
    /// <param name="pageSize">The pageSize for pagination. Optional.</param>
    /// <param name="ct">The CancellationToken for the operation.</param>
    /// <returns>An async operation with the filtered TelemetryReading entries as a view model.</returns>
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
