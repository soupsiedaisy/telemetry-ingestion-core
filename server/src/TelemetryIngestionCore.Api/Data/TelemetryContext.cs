using Microsoft.EntityFrameworkCore;
using TelemetryIngestionCore.Api.Models;

namespace TelemetryIngestionCore.Api.Data;

/// <summary>
/// The DbContext used by EntityFramework to interact with the database.
/// </summary>
/// <param name="options">The DbContextOptions injected by the program.</param>
public class TelemetryContext(DbContextOptions<TelemetryContext> options) : DbContext(options)
{
    /// <summary>
    /// The DbSet used by EntityFramework to track the TelemetryReading entries in the database.
    /// </summary>
    public DbSet<TelemetryReading> TelemetryReadings { get; set; }

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
}
