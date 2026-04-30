namespace TelemetryIngestionCore.Api;

/// <summary>
/// Application entry point.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Main entry method. Builds and runs the web host.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    internal static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    /// <summary>
    /// Creates and configures the <see cref="IHostBuilder"/> for the application.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>A configured <see cref="IHostBuilder"/>.</returns>
    internal static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
}
