using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TelemetryIngestionCore.Api.Data;

namespace TelemetryIngestionCore.Tests.Contexts;

public class TestTelemetryContext(
    SqliteConnection connection,
    IConfiguration configuration,
    DbContextOptions<TelemetryContext> options
) : TelemetryContext(configuration, options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite(connection);
}
