using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TelemetryIngestionCore.Api.Models;

namespace TelemetryIngestionCore.Api.Data;

public class TelemetryContext(
    IConfiguration configuration,
    DbContextOptions<TelemetryContext> options
) : DbContext(options)
{
    public DbSet<TelemetryReading> TelemetryReadings { get; set; }
    public string ConnectionString { get; } =
        configuration.GetConnectionString("TelemetryDb") ?? "";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TelemetryReading>(builder =>
        {
            builder.ToTable("TelemetryReadings");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).ValueGeneratedNever(); // rather using client side guid generation

            builder.Property(e => e.TenantId).IsRequired().HasMaxLength(200);

            builder.Property(e => e.ExternalId).HasMaxLength(200);

            builder.Property(e => e.DeviceId).IsRequired().HasMaxLength(200);

            builder.Property(e => e.Type).IsRequired().HasMaxLength(200);

            builder.Property(e => e.Unit).IsRequired().HasMaxLength(50);

            builder.Property(e => e.Value).IsRequired();

            builder.Property(e => e.Battery).IsRequired();
            builder.Property(e => e.Signal).IsRequired();

            builder
                .Property(e => e.RecordedAt)
                .HasConversion(
                    date => date.ToUnixTimeMilliseconds(),
                    time => DateTimeOffset.FromUnixTimeMilliseconds(time)
                )
                .IsRequired();

            builder
                .Property(e => e.CreatedAt)
                .HasConversion(
                    date => date.ToUnixTimeMilliseconds(),
                    time => DateTimeOffset.FromUnixTimeMilliseconds(time)
                )
                .IsRequired();

            // Indexes to optimize common queries
            builder.HasIndex(e => e.TenantId);
            builder.HasIndex(e => e.DeviceId);
            builder.HasIndex(e => e.RecordedAt);

            // SQLite doesn't support filtered unique indexes via EF migrations directly.
            // Approach: enforce uniqueness in application logic/transaction.
            // We'll add a regular index here; prefer adding the filtered-unique index in a migration with SQL for production.
            builder
                .HasIndex(e => new { e.TenantId, e.ExternalId })
                .HasDatabaseName("IX_TelemetryReadings_TenantId_ExternalId ");
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connection = new SqliteConnection(ConnectionString);

        optionsBuilder.UseSqlite(connection);
    }
}
