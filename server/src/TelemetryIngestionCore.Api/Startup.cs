using TelemetryIngestionCore.Api.Configuration;
using TelemetryIngestionCore.Api.Endpoints;

namespace TelemetryIngestionCore.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services) =>
        services
            .Configure<AppOptions>(configuration.GetSection("AppOptions"))
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger().UseSwaggerUI();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapHealthEndpoints());
    }
}
