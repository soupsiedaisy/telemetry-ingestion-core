namespace TelemetryIngestionCore.Api.Configuration;

/// <summary>
/// Configurable options used throughout the application.
/// </summary>
public class AppOptions
{
    /// <summary>
    /// The threshold used for calculating the batteryLow status object. (default = 20)
    /// </summary>
    public int BatteryLowThreshold { get; set; } = 20;

    /// <summary>
    /// The maximum page size allowed in pagination from the query endpoint. (default = 500)
    /// </summary>
    public int MaxPageSize { get; set; } = 500;

    /// <summary>
    /// The default page size for pagination from the query endpoint. (default = 50)
    /// </summary>
    public int DefaultPageSize { get; set; } = 50;
}
