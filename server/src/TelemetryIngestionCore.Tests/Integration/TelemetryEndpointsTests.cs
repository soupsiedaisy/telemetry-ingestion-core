using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TelemetryIngestionCore.Api;
using TelemetryIngestionCore.Api.Data;
using TelemetryIngestionCore.Api.Models;
using TelemetryIngestionCore.Tests.Factories;

namespace TelemetryIngestionCore.Tests.Integration;

public class TelemetryEndpointsTests : IClassFixture<TelemetryWebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;
    private readonly TelemetryContext _context;
    private readonly IServiceScope _scope;

    public TelemetryEndpointsTests(TelemetryWebApplicationFactory<Startup> factory)
    {
        _client = factory.CreateDefaultClient();
        _scope = factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<TelemetryContext>();
        _context.Database.EnsureCreated();
    }

    [Fact(DisplayName = "Happy flow - create telemetry then query returns created item")]
    public async Task CreateThenQuery_HappyFlow_ReturnsCreatedAndFound()
    {
        var dto = new TelemetryDto
        {
            TenantId = "tenant-int-test",
            ExternalId = "ext-happy-1",
            DeviceId = "device-1",
            Type = "temperature",
            Unit = "C",
            Value = 21.5,
            Battery = 77,
            Signal = -65,
            RecordedAt = DateTimeOffset.UtcNow,
        };

        var createResp = (
            await _client.PostAsJsonAsync("api/telemetry", dto)
        ).EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);

        var created = await createResp.Content.ReadFromJsonAsync<TelemetryView>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(created);
        Assert.Equal(dto.TenantId, created.TenantId);
        Assert.Equal(dto.ExternalId, created.ExternalId);
        Assert.Equal(dto.DeviceId, created.DeviceId);
        Assert.Equal(dto.Type, created.Type);
        Assert.Equal(dto.Unit, created.Unit);
        Assert.Equal(dto.Value, created.Value);
        Assert.Equal(dto.Battery, created.Battery);
        Assert.Equal(dto.Signal, created.Signal);

        var queryUri =
            $"api/telemetry?tenantId={WebUtility.UrlEncode(dto.TenantId)}&deviceId={WebUtility.UrlEncode(dto.DeviceId)}";

        var queryResp = (await _client.GetAsync(queryUri)).EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.OK, queryResp.StatusCode);

        var list = await queryResp.Content.ReadFromJsonAsync<TelemetryView[]>();

        Assert.NotNull(list);

        Assert.Single(
            list,
            v =>
                dto.TenantId == v.TenantId
                && dto.ExternalId == v.ExternalId
                && dto.DeviceId == v.DeviceId
                && dto.Type == v.Type
                && dto.Unit == v.Unit
                && dto.Value == v.Value
                && dto.Battery == v.Battery
                && dto.Signal == v.Signal
        );
    }

    [Fact(DisplayName = "Failure flow - create with invalid DTO returns validation problem")]
    public async Task Create_InvalidDto_ReturnsValidationProblem()
    {
        var invalidDto = new
        {
            TenantId = "tenant-int-test",
            ExternalId = "ext-unhappy-1",
            DeviceId = "device-1",
            Type = "temperature",
            Unit = "C",
            Value = 10.0,
            Battery = 150, // out of valid range
            Signal = -50,
            RecordedAt = DateTimeOffset.UtcNow,
        };

        var resp = await _client.PostAsJsonAsync("api/telemetry", invalidDto);

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);

        var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Object, body.ValueKind);

        bool hasErrors =
            body.TryGetProperty("errors", out _)
            || body.EnumerateObject().Any();
        Assert.True(hasErrors);
    }

    [Fact(DisplayName = "Failure flow - create duplicate external id returns 409 Conflict")]
    public async Task Create_DuplicateExternalId_ReturnsConflict()
    {
        var dto = new TelemetryDto
        {
            TenantId = "tenant-dup-test",
            ExternalId = "ext-dup-1",
            DeviceId = "device-dup",
            Type = "humidity",
            Unit = "%",
            Value = 55.0,
            Battery = 50,
            Signal = -70,
            RecordedAt = DateTimeOffset.UtcNow,
        };

        var first = await _client.PostAsJsonAsync("api/telemetry", dto);
        first.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await _client.PostAsJsonAsync("api/telemetry", dto);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);

        var body = await second.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Object, body.ValueKind);
        Assert.True(body.TryGetProperty("error", out var _));
    }
}
