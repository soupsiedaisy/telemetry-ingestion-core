using System.Net;
using TelemetryIngestionCore.Api;
using TelemetryIngestionCore.Tests.Factories;

namespace TelemetryIngestionCore.Tests.Integration;

public class HealthEndpointsTests(TelemetryWebApplicationFactory<Startup> factory)
    : IClassFixture<TelemetryWebApplicationFactory<Startup>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task LiveEndpoint_ReturnsOk()
    {
        var resp = await _client.GetAsync("/health/live");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task ReadyEndpoint_ReturnsOk()
    {
        var resp = await _client.GetAsync("/health/ready");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
