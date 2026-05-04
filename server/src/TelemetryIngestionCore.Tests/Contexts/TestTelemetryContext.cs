using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TelemetryIngestionCore.Api.Data;

namespace TelemetryIngestionCore.Tests.Contexts;

public class TestTelemetryContext(
    SqliteConnection connection,
    DbContextOptions<TelemetryContext> options
) : TelemetryContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite(connection);
}
