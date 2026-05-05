using TelemetryIngestionCore.Api.Models;

namespace TelemetryIngestionCore.Api.Data;

/// <summary>
/// A repository responsible for transactions with the database via the TelemetryContext.
/// </summary>
public interface ITelemetryRepository
{
    /// <summary>
    /// Creates a TelemetryReading entry in the database.
    /// </summary>
    /// <param name="reading">The TelemetryReading to create in the database. Required.</param>
    /// <param name="ct">The CancellationToken for the operation.</param>
    /// <returns>An async operation with the created TelemetryReading.</returns>
    public abstract Task<TelemetryReading> CreateAsync(
        TelemetryReading reading,
        CancellationToken ct = default
    );

    /// <summary>
    /// Queries the TelemetryReading entries in the database.
    /// </summary>
    /// <param name="tenantId">The TelemetryReading tenantId to filter the responses with. Optional.</param>
    /// <param name="deviceId">The TelemetryReading deviceId to filter the responses with. Optional.</param>
    /// <param name="type">The TelemetryReading type to filter the responses with. Optional.</param>
    /// <param name="from">A 'from' date to filter the TelemetryReading responses with, based on the recordedAt property. Optional.</param>
    /// <param name="to">A 'to' date to filter the TelemetryReading responses with, based on the recordedAt property. Optional.</param>
    /// <param name="page">The page number for pagination. Optional.</param>
    /// <param name="pageSize">The pageSize for pagination. Optional.</param>
    /// <param name="ct">The CancellationToken for the operation.</param>
    /// <returns>The queried TelemetryReading entries and pagination metadata.</returns>
    public abstract Task<TelemetryReadingsPaginationData> QueryAsync(
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
