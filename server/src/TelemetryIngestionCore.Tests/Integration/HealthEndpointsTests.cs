using System.Net;
using Microsoft.Extensions.DependencyInjection;
using TelemetryIngestionCore.Api;
using TelemetryIngestionCore.Api.Data;
using TelemetryIngestionCore.Tests.Factories;

namespace TelemetryIngestionCore.Tests.Integration;

public class HealthEndpointsTests : IClassFixture<TelemetryWebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;
    private readonly TelemetryContext _context;
    private readonly IServiceScope _scope;

    public HealthEndpointsTests(TelemetryWebApplicationFactory<Startup> factory)
    {
        _client = factory.CreateDefaultClient();
        _scope = factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<TelemetryContext>();
        _context.Database.EnsureCreated();
    }

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
