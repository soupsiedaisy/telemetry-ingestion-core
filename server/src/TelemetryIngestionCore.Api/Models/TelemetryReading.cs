namespace TelemetryIngestionCore.Api.Models;

public class TelemetryReading
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TenantId { get; set; } = null!;
    public string? ExternalId { get; set; }
    public string DeviceId { get; set; } = null!;
    public string Type { get; set; } = null!;
    public double Value { get; set; }
    public string Unit { get; set; } = null!;
    public int Battery { get; set; }
    public int Signal { get; set; }
    public DateTimeOffset RecordedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
