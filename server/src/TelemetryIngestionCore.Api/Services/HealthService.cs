using Microsoft.EntityFrameworkCore;
using TelemetryIngestionCore.Api.Data;

namespace TelemetryIngestionCore.Api.Services;

public class HealthService(TelemetryContext context) : IHealthService
{
    public bool CheckDb()
    {
        try
        {
            context.Database.ExecuteSqlRaw("SELECT 1");
            return true;
        }
        catch
        {
            return false;
        }
    }
}
