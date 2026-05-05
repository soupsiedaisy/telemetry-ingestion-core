using System.ComponentModel.DataAnnotations;
using TelemetryIngestionCore.Api.Models;

namespace TelemetryIngestionCore.Tests.Unit;

public class TelemetryDtoValidationTests
{
    private static bool Validate(object model, out IList<ValidationResult> results)
    {
        var ctx = new ValidationContext(model, serviceProvider: null, items: null);

        results = [];

        return Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
    }

    private TelemetryDto CreateValidDto() =>
        new()
        {
            TenantId = "tenant-1",
            ExternalId = "external-1",
            DeviceId = "device-1",
            Type = "temperature",
            Value = 23.5,
            Unit = "C",
            Battery = 75,
            Signal = -50,
            RecordedAt = DateTimeOffset.UtcNow,
        };

    [Fact]
    public void ValidDto_PassesValidation()
    {
        var dto = CreateValidDto();

        var isValid = Validate(dto, out var results);

        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void Battery_OutOfRange_FailsValidation()
    {
        var dto = CreateValidDto();
        dto.Battery = 200; // out of [0,100]

        var isValid = Validate(dto, out var results);

        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Battery)));
    }

    [Fact]
    public void Signal_OutOfRange_FailsValidation()
    {
        var dto = CreateValidDto();
        dto.Signal = 1; // out of [-200,0]

        var isValid = Validate(dto, out var results);

        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Signal)));
    }

    [Fact]
    public void TenantId_OutsideMaxLength_FailsValidation()
    {
        var dto = CreateValidDto();
        dto.TenantId = string.Concat(Enumerable.Repeat("a", 201)); // outside of max length of 200

        var isValid = Validate(dto, out var results);

        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.TenantId)));
    }
}
