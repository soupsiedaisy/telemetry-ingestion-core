namespace TelemetryIngestionCore.Api.Endpoints;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/health/live", () => Results.Ok());

        return builder;
    }
}
