using TelemetryIngestionCore.Api.Services;

namespace TelemetryIngestionCore.Api.Endpoints;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/health/live", () => Results.Ok());
        builder.MapGet(
            "/health/ready",
            (IHealthService healthService) =>
                healthService.CheckDb() ? Results.Ok() : Results.Problem(statusCode: 503)
        );

        return builder;
    }
}
