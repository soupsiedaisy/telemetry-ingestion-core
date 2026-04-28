namespace TelemetryIngestionCore.Api.Configuration;

public class AppOptions
{
    public int BatteryLowThreshold { get; set; } = 20;

    public int MaxPageSize { get; set; } = 500;

    public int DefaultPageSize { get; set; } = 50;
}
