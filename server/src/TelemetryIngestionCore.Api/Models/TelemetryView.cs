namespace TelemetryIngestionCore.Api.Models;

/// <summary>
/// View model used by the API layer when responding with telemetry readings.
/// </summary>
public class TelemetryView
{
    /// <inheritdoc cref="TelemetryReading.TenantId"></inheritdoc>
    public string TenantId { get; set; } = null!;

    /// <inheritdoc cref="TelemetryReading.ExternalId"></inheritdoc>
    public string? ExternalId { get; set; }

    /// <inheritdoc cref="TelemetryReading.DeviceId"></inheritdoc>
    public string DeviceId { get; set; } = null!;

    /// <inheritdoc cref="TelemetryReading.Type"></inheritdoc>
    public string Type { get; set; } = null!;

    /// <inheritdoc cref="TelemetryReading.Unit"></inheritdoc>
    public string Unit { get; set; } = null!;

    /// <inheritdoc cref="TelemetryReading.Value"></inheritdoc>
    public double Value { get; set; }

    /// <inheritdoc cref="TelemetryReading.Battery"></inheritdoc>
    public int Battery { get; set; }

    /// <summary>
    /// Computed status object indicating the device's battery is low.
    /// </summary>
    public bool BatteryLow { get; set; }

    /// <inheritdoc cref="TelemetryReading.Signal"></inheritdoc>
    public int Signal { get; set; }

    /// <inheritdoc cref="TelemetryReading.RecordedAt"></inheritdoc>
    public string RecordedAt { get; set; } = null!;

    /// <inheritdoc cref="TelemetryReading.CreatedAt"></inheritdoc>
    public string CreatedAt { get; set; } = null!;
}
