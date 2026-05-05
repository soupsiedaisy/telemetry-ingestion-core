namespace TelemetryIngestionCore.Api.Models;

/// <summary>
/// Model for responses that contain telemetry readings and pagination
/// </summary>
public class TelemetryPaginationView
{
    /// <summary>
    /// The queried telemetry readings.
    /// </summary>
    public required IEnumerable<TelemetryView> TelemetryReadings { get; set; }

    /// <summary>
    /// Metadata about pagination.
    /// </summary>
    public required PaginationMetadata PaginationMetadata { get; set; }
}
