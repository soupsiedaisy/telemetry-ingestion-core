using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TelemetryIngestionCore.Api.Configuration;
using TelemetryIngestionCore.Api.Data;
using TelemetryIngestionCore.Api.Exceptions;
using TelemetryIngestionCore.Api.Models;
using TelemetryIngestionCore.Tests.Contexts;

namespace TelemetryIngestionCore.Tests.Unit;

public class TelemetryRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<TelemetryContext> _contextOptions;

    public TelemetryRepositoryTests()
    {
        _connection = new SqliteConnection("Data source=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<TelemetryContext>().Options;

        using var ctx = new TestTelemetryContext(_connection, _contextOptions);

        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }

    private TelemetryContext CreateContext() =>
        new TestTelemetryContext(_connection, _contextOptions);

    private TelemetryRepository CreateRepository(TelemetryContext context, AppOptions options) =>
        new(context, Options.Create(options));

    private TelemetryRepository CreateRepository(TelemetryContext context) =>
        CreateRepository(
            context,
            new()
            {
                MaxPageSize = 500,
                DefaultPageSize = 50,
                BatteryLowThreshold = 20,
            }
        );

    private TelemetryReading CreateReading(
        string tenantId = "tenant-1",
        string deviceId = "device-A",
        string externalId = "ext-1",
        string type = "temp",
        string unit = "m",
        double value = 12.34,
        int battery = 50,
        int signal = -100,
        DateTimeOffset? recordedAt = null
    ) =>
        new()
        {
            Id = Guid.Empty, // repository should set this
            TenantId = tenantId,
            DeviceId = deviceId,
            ExternalId = externalId,
            Type = type,
            Unit = unit,
            Value = value,
            Battery = battery,
            Signal = signal,
            RecordedAt = recordedAt ?? DateTimeOffset.UtcNow,
            CreatedAt = default,
        };

    [Fact]
    public async Task CreateAsync_NullReading_ThrowsArgumentNullException()
    {
        await using var context = CreateContext();

        var repo = CreateRepository(context);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            repo.CreateAsync(null!, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateAsync_DuplicateExternalIdForTenantId_ThrowsDuplicateExternalIdException()
    {
        await using var context = CreateContext();

        var repo = CreateRepository(context);

        var readings = await context.TelemetryReadings.ToListAsync();

        Assert.Empty(readings);

        const string tenantId = "tenant-1";
        const string externalId = "ext-1";

        var reading = CreateReading(tenantId: tenantId, externalId: externalId);

        var result = await repo.CreateAsync(reading);

        var sameReading = CreateReading(tenantId: tenantId, externalId: externalId);

        await Assert.ThrowsAsync<DuplicateExternalIdException>(() =>
            repo.CreateAsync(sameReading, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateAsync_AssignsIdAndCreatedAt_AndPersists()
    {
        await using var context = CreateContext();

        var repo = CreateRepository(context);

        var reading = CreateReading();

        var result = await repo.CreateAsync(reading);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default, result.CreatedAt);

        var persisted = await context.TelemetryReadings.FindAsync(result.Id);

        Assert.NotNull(persisted);
        Assert.Equal(result.TenantId, persisted.TenantId);
        Assert.Equal(result.ExternalId, persisted.ExternalId);
    }

    [Fact]
    public async Task QueryAsync_EmptyQueryWithEmptyDb_ReturnsEmptySet()
    {
        await using var context = CreateContext();

        var repo = CreateRepository(context);

        var results = await repo.QueryAsync();

        Assert.Empty(results);
    }

    [Fact]
    public async Task QueryAsync_AllFilters_ReturnsExpectedSet()
    {
        await using var context = CreateContext();

        var repo = CreateRepository(context);

        var now = DateTimeOffset.FromUnixTimeMilliseconds(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

        var readings = new[]
        {
            // inside filter
            CreateReading(
                tenantId: "t1",
                deviceId: "d1",
                externalId: "e1",
                type: "A",
                recordedAt: now.AddMinutes(-5)
            ),
            CreateReading(
                tenantId: "t1",
                deviceId: "d1",
                externalId: "e2",
                type: "A",
                recordedAt: now.AddMinutes(-4)
            ),
            CreateReading(
                tenantId: "t1",
                deviceId: "d1",
                externalId: "e3",
                type: "A",
                recordedAt: now.AddMinutes(-3)
            ),
            // outside filter
            CreateReading(
                tenantId: "t2",
                deviceId: "d1",
                externalId: "e4",
                type: "A",
                recordedAt: now.AddMinutes(-5)
            ),
            CreateReading(
                tenantId: "t1",
                deviceId: "d2",
                externalId: "e5",
                type: "A",
                recordedAt: now.AddMinutes(-4)
            ),
            CreateReading(
                tenantId: "t1",
                deviceId: "d1",
                externalId: "e6",
                type: "B",
                recordedAt: now.AddMinutes(-3)
            ),
            CreateReading(
                tenantId: "t1",
                deviceId: "d1",
                externalId: "e7",
                type: "A",
                recordedAt: now.AddMinutes(-6)
            ),
            CreateReading(
                tenantId: "t1",
                deviceId: "d1",
                externalId: "e8",
                type: "A",
                recordedAt: now.AddMinutes(-2)
            ),
        };

        foreach (var r in readings)
        {
            r.Id = Guid.NewGuid();
            r.CreatedAt = DateTimeOffset.UtcNow;
            context.TelemetryReadings.Add(r);
        }

        await context.SaveChangesAsync();

        const string tenantIdFilter = "t1";
        const string deviceIdFilter = "d1";
        const string typeFilter = "A";
        var fromFilter = now.AddMinutes(-5);
        var toFilter = now.AddMinutes(-3);

        var results = await repo.QueryAsync(
            tenantId: tenantIdFilter,
            deviceId: deviceIdFilter,
            type: typeFilter,
            from: fromFilter,
            to: toFilter
        );

        Assert.All(results, r => Assert.Equal(tenantIdFilter, r.TenantId));
        Assert.All(results, r => Assert.Equal(deviceIdFilter, r.DeviceId));
        Assert.All(results, r => Assert.Equal(typeFilter, r.Type));
        Assert.All(
            results,
            r => Assert.True(DateTimeOffset.Compare(fromFilter, r.RecordedAt) <= 0)
        );
        Assert.All(results, r => Assert.True(DateTimeOffset.Compare(r.RecordedAt, toFilter) <= 0));
    }

    [Fact]
    public async Task QueryAsync_Pagination_ReturnsExpectedSet()
    {
        await using var context = CreateContext();

        var repo = CreateRepository(context);

        var now = DateTimeOffset.UtcNow;

        var readings = new[]
        {
            CreateReading(deviceId: "page1", externalId: "e1", recordedAt: now.AddMinutes(-1)),
            CreateReading(deviceId: "page1", externalId: "e2", recordedAt: now.AddMinutes(-2)),
            CreateReading(deviceId: "page2", externalId: "e3", recordedAt: now.AddMinutes(-3)),
            CreateReading(deviceId: "page2", externalId: "e4", recordedAt: now.AddMinutes(-4)),
            // expected results start, with page=3 and pageSize=2
            CreateReading(deviceId: "page3", externalId: "e5", recordedAt: now.AddMinutes(-5)),
            CreateReading(deviceId: "page3", externalId: "e6", recordedAt: now.AddMinutes(-6)),
            // expected results end
            CreateReading(deviceId: "page4", externalId: "e7", recordedAt: now.AddMinutes(-7)),
            CreateReading(deviceId: "page4", externalId: "e8", recordedAt: now.AddMinutes(-8)),
        };

        foreach (var r in readings)
        {
            r.Id = Guid.NewGuid();
            r.CreatedAt = now;
            context.TelemetryReadings.Add(r);
        }

        await context.SaveChangesAsync();

        var results = await repo.QueryAsync(page: 3, pageSize: 2);

        Assert.Equal(2, results.Count);
        Assert.All(results, r => Assert.Equal("page3", r.DeviceId));
    }
}
