using TelemetryIngestionCore.Api.Configuration;
using TelemetryIngestionCore.Api.Data;
using TelemetryIngestionCore.Api.Endpoints;
using TelemetryIngestionCore.Api.Services;

namespace TelemetryIngestionCore.Api;

/// <summary>
/// Configures services and the application for the Telemetry Ingestion API.
/// </summary>
/// <param name="configuration">Application configuration injected by the host.</param>
public class Startup(IConfiguration configuration)
{
    /// <summary>
    /// Register services with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    public void ConfigureServices(IServiceCollection services) =>
        services
            .Configure<AppOptions>(configuration.GetSection("AppOptions"))
            .AddDbContext<TelemetryContext>()
            .AddScoped<ITelemetryRepository, TelemetryRepository>()
            .AddScoped<ITelemetryService, TelemetryService>()
            .AddScoped<IHealthService, HealthService>()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

    /// <summary>
    /// Configures the application and uses relevent environment values.
    /// </summary>
    /// <param name="app">The application builder used to configure middleware.</param>
    /// <param name="env">Hosting environment information.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger().UseSwaggerUI();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapHealthEndpoints().MapTelemetryEndpoints());
    }
}
