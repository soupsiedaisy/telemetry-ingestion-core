using System.ComponentModel.DataAnnotations;

namespace TelemetryIngestionCore.Api.Models;

public class TelemetryDto
{
    [Required]
    [MaxLength(200)]
    public string TenantId { get; set; } = null!;

    [MaxLength(200)]
    public string? ExternalId { get; set; }

    [Required]
    [MaxLength(200)]
    public string DeviceId { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Type { get; set; } = null!;

    [Required]
    public double Value { get; set; }

    [Required]
    [MaxLength(50)]
    public string Unit { get; set; } = null!;

    [Required]
    [Range(0, 100)]
    public int Battery { get; set; }

    [Required]
    [Range(-200, 0)]
    public int Signal { get; set; }

    [Required]
    public DateTimeOffset RecordedAt { get; set; }
}
