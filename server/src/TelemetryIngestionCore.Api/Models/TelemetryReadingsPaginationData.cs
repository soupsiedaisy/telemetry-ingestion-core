namespace TelemetryIngestionCore.Api.Models;

/// <summary>
/// Model for the repository to return telemetry readings and pagination data
/// </summary>
public class TelemetryReadingsPaginationData
{
    /// <summary>
    /// The queried telemetry readings.
    /// </summary>
    public required IReadOnlyList<TelemetryReading> TelemetryReadings { get; set; }

    /// <summary>
    /// Metadata about pagination.
    /// </summary>
    public required PaginationMetadata PaginationMetadata { get; set; }
}
