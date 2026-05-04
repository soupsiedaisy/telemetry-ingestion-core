namespace TelemetryIngestionCore.Api.Models;

/// <summary>
/// Model used by Entity Framework to represent the telemetry readings in the database.
/// </summary>
public class TelemetryReading
{
    /// <summary>
    /// Unique identifier generated for every new reading.
    /// Used as the primary key in the TelemetryReadings database table.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Tenant identifier owning the telemetry reading.
    /// </summary>
    public string TenantId { get; set; } = null!;

    /// <summary>
    /// External identifier provided by the device or upstream system.
    /// Used to detect duplicates within a tenant.
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// Device identifier that produced the reading.
    /// </summary>
    public string DeviceId { get; set; } = null!;

    /// <summary>
    /// Measurement type (for example "temperature", "water_level", etc.).
    /// </summary>
    public string Type { get; set; } = null!;

    /// <summary>
    /// Numeric measurement value for the reading.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Unit of measurement for the reading (for example "C", "m", "%", etc.).
    /// </summary>
    public string Unit { get; set; } = null!;

    /// <summary>
    /// Battery level reported by the device as a percentage.
    /// </summary>
    public int Battery { get; set; }

    /// <summary>
    /// Signal strength reported by the device.
    /// Expects negative dBm values (e.g. -70).
    /// </summary>
    public int Signal { get; set; }

    /// <summary>
    /// Timestamp when the measurement was recorded on the device.
    /// </summary>
    public DateTimeOffset RecordedAt { get; set; }

    /// <summary>
    /// Timestamp when the measurement was created in the database.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
