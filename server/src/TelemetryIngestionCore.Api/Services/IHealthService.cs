namespace TelemetryIngestionCore.Api.Services;

/// <summary>
/// A simple health service for checking application readiness.
/// </summary>
public interface IHealthService
{
    /// <summary>
    /// A simple database health check.
    /// </summary>
    /// <returns>True if the database is connected to and readable by the application.</returns>
    public abstract bool CheckDb();
}
