using TelemetryIngestionCore.Api.Endpoints;

namespace TelemetryIngestionCore.Api;

public class Startup
{
    public void ConfigureServices(IServiceCollection services) =>
        services
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
