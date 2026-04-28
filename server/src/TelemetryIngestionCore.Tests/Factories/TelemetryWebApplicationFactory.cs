using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace TelemetryIngestionCore.Tests.Factories;

public class TelemetryWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(
            (_, config) =>
            {
                var inMemory = new Dictionary<string, string?>
                {
                    ["ConnectionStrings:TelemetryDb"] = "DataSource=:memory:",
                };
                config.AddInMemoryCollection(inMemory);
            }
        );

        builder.UseEnvironment("Development");
    }
}
