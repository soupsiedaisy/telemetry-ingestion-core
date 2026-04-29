using TelemetryIngestionCore.Api.Configuration;
using TelemetryIngestionCore.Api.Data;
using TelemetryIngestionCore.Api.Endpoints;
using TelemetryIngestionCore.Api.Services;

namespace TelemetryIngestionCore.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services) =>
        services
            .Configure<AppOptions>(configuration.GetSection("AppOptions"))
            .AddDbContext<TelemetryContext>()
            .AddScoped<ITelemetryRepository, TelemetryRepository>()
            .AddScoped<ITelemetryService, TelemetryService>()
            .AddScoped<IHealthService, HealthService>()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

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
