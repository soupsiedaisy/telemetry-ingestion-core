using TelemetryIngestionCore.Api.Services;

namespace TelemetryIngestionCore.Api.Endpoints;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("/health/live", () => Results.Ok())
            .WithName("HealthLive")
            .Produces(statusCode: 200);
        builder
            .MapGet(
                "/health/ready",
                (IHealthService healthService) =>
                    healthService.CheckDb() ? Results.Ok() : Results.Problem(statusCode: 503)
            )
            .WithName("HealthReady")
            .Produces(200)
            .Produces(503);

        return builder;
    }
}
