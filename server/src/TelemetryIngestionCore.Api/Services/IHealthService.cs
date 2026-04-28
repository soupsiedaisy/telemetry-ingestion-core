namespace TelemetryIngestionCore.Api.Services;

public interface IHealthService
{
    public abstract bool CheckDb();
}
