using System.ComponentModel.DataAnnotations;

namespace TelemetryIngestionCore.Api.Models;

/// <summary>
/// Data transfer object used by the API layer when creating a telemetry reading.
/// </summary>
public class TelemetryDto
{
    /// <remarks>
    /// Required. Max length 200.
    /// </remarks>
    /// <inheritdoc cref="TelemetryReading.TenantId"></inheritdoc>
    [Required]
    [MaxLength(200)]
    public string TenantId { get; set; } = null!;

    /// <remarks>
    /// Max length 200.
    /// </remarks>
    /// <inheritdoc cref="TelemetryReading.ExternalId"></inheritdoc>
    [MaxLength(200)]
    public string? ExternalId { get; set; }

    /// <remarks>
    /// Required. Max length 200.
    /// </remarks>
    /// <inheritdoc cref="TelemetryReading.DeviceId"></inheritdoc>
    [Required]
    [MaxLength(200)]
    public string DeviceId { get; set; } = null!;

    /// <remarks>
    /// Required. Max length 200.
    /// </remarks>
    /// <inheritdoc cref="TelemetryReading.Type"></inheritdoc>
    [Required]
    [MaxLength(200)]
    public string Type { get; set; } = null!;

    /// <remarks>
    /// Required.
    /// </remarks>
    /// <inheritdoc cref="TelemetryReading.Value"></inheritdoc>
    [Required]
    public double Value { get; set; }

    /// <remarks>
    /// Required. Max length 50.
    /// </remarks>
    /// <inheritdoc cref="TelemetryReading.Unit"></inheritdoc>
    [Required]
    [MaxLength(50)]
    public string Unit { get; set; } = null!;

    /// <remarks>
    /// Required. Valid range 0 to 100.
    /// </remarks>
    /// <inheritdoc cref="TelemetryReading.Battery"></inheritdoc>
    [Required]
    [Range(0, 100)]
    public int Battery { get; set; }

    /// <remarks>
    /// Required. Valid range -200 to 0.
    /// </remarks>
    /// <inheritdoc cref="TelemetryReading.Signal"></inheritdoc>
    [Required]
    [Range(-200, 0)]
    public int Signal { get; set; }

    /// <remarks>
    /// Required. Use UTC (DateTimeOffset) where possible.
    /// </remarks>
    /// <inheritdoc cref="TelemetryReading.RecordedAt"></inheritdoc>
    [Required]
    public DateTimeOffset RecordedAt { get; set; }
}
