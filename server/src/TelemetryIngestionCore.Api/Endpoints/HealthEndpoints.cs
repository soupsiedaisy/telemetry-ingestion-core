using TelemetryIngestionCore.Api.Services;

namespace TelemetryIngestionCore.Api.Endpoints;

/// <summary>
/// Static class used to map the health checks for the application.
/// </summary>
public static class HealthEndpoints
{
    /// <summary>
    /// Extends IEndpointRouteBuilder instances with this method to map the health endpoints for the application.
    /// </summary>
    /// <returns>The IEndpointRouteBuilder used for defining route building contracts.</returns>
    /// <param name="builder">The IEndpointRouteBuilder used for defining route building contracts.</param>
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
