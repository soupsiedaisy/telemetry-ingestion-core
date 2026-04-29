namespace TelemetryIngestionCore.Api.Models;

public class TelemetryView
{
    public string TenantId { get; set; } = null!;
    public string? ExternalId { get; set; }
    public string DeviceId { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Unit { get; set; } = null!;
    public double Value { get; set; }
    public int Battery { get; set; }
    public bool BatteryLow { get; set; }
    public int Signal { get; set; }
    public string RecordedAt { get; set; } = null!;
    public string CreatedAt { get; set; } = null!;
}
