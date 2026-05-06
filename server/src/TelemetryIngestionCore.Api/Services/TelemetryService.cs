using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.Extensions.Options;
using TelemetryIngestionCore.Api.Configuration;
using TelemetryIngestionCore.Api.Data;
using TelemetryIngestionCore.Api.Exceptions;
using TelemetryIngestionCore.Api.Models;

namespace TelemetryIngestionCore.Api.Services;

/// <param name="repository">The TelemetryRepository injected by the program.</param>
/// <inheritdoc cref="ITelemetryService" />
public class TelemetryService(ITelemetryRepository repository, ILogger<TelemetryService> logger)
    : ITelemetryService
{
    /// <summary>
    /// The threshold used for calculating the batteryLow status object.
    /// </summary>
    private readonly int batteryLowThreshold;

    /// <summary>
    /// The maximum page size allowed in pagination from the query endpoint.
    /// </summary>
    private readonly int maxPageSize;

    /// <summary>
    /// Contstructor for creating a TelemetryService instance.
    /// </summary>
    /// <param name="repository">The TelemetryRepository injected by the program.</param>
    /// <param name="options">The IOptions app options injected by the program.</param>
    public TelemetryService(
        ITelemetryRepository repository,
        ILogger<TelemetryService> logger,
        IOptions<AppOptions> options
    )
        : this(repository, logger)
    {
        batteryLowThreshold = options.Value.BatteryLowThreshold;
        maxPageSize = options.Value.MaxPageSize;
    }

    /// <exception cref="ArgumentNullException">The exception thrown when the method is passed a null TelemetryDto.</exception>
    /// <inheritdoc cref="ITelemetryService.CreateReadingAsync(TelemetryDto, CancellationToken)" />
    public async Task<TelemetryView> CreateReadingAsync(
        TelemetryDto dto,
        CancellationToken ct = default
    )
    {
        if (dto == null)
        {
            logger.LogError("CreateReadingAsync called with null dto");

            throw new ArgumentNullException(nameof(dto));
        }

        try
        {
            var validationContext = new ValidationContext(dto, serviceProvider: null, items: null);

            Validator.ValidateObject(dto, validationContext, validateAllProperties: true);
        }
        catch (ValidationException vex)
        {
            logger.LogWarning(
                vex,
                "CreateReadingAsync called with invalid DTO for TenantId={TenantId}, DeviceId={DeviceId}, ExternalId={ExternalId}",
                dto.TenantId,
                dto.DeviceId,
                dto.ExternalId
            );

            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "CreateReadingAsync threw an exception with TenantId={TenantId}, DeviceId={DeviceId}, ExternalId={ExternalId}",
                dto.TenantId,
                dto.DeviceId,
                dto.ExternalId
            );

            throw;
        }

        var reading = new TelemetryReading
        {
            Id = Guid.Empty,
            TenantId = dto.TenantId,
            ExternalId = dto.ExternalId ?? string.Empty,
            DeviceId = dto.DeviceId,
            Type = dto.Type,
            Unit = dto.Unit,
            Value = dto.Value,
            Battery = dto.Battery,
            Signal = dto.Signal,
            RecordedAt = dto.RecordedAt,
            CreatedAt = default,
        };

        TelemetryReading? created;

        // TODO this process can probably be abstracted
        try
        {
            created = await repository.CreateAsync(reading, ct).ConfigureAwait(false);
        }
        catch (DuplicateExternalIdException dex)
        {
            logger.LogError(
                dex,
                "CreateReadingAsync called with duplicate ExternalId={ExternalId} for TenantId={TenantId}, with DeviceId={DeviceId}",
                dto.TenantId,
                dto.DeviceId,
                dto.ExternalId
            );

            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "CreateReadingAsync threw an exception with TenantId={TenantId}, DeviceId={DeviceId}, ExternalId={ExternalId}",
                dto.TenantId,
                dto.DeviceId,
                dto.ExternalId
            );

            throw;
        }

        logger.LogInformation(
            "CreateReadingAsync created a reading with TenantId={TenantId}, DeviceId={DeviceId}, ExternalId={ExternalId}",
            dto.TenantId,
            dto.DeviceId,
            dto.ExternalId
        );

        return MapToView(created);
    }

    /// <exception cref="ArgumentException">The exception thrown when the method is passed a pageSize greater than the maximum configured page size.</exception>
    /// <inheritdoc cref="ITelemetryService.QueryReadingsAsync(string?, string?, string?, DateTimeOffset?, DateTimeOffset?, int?, int?, CancellationToken)" />
    public async Task<TelemetryPaginationView> QueryReadingsAsync(
        string? tenantId = null,
        string? deviceId = null,
        string? type = null,
        string? from = null,
        string? to = null,
        int? page = null,
        int? pageSize = null,
        CancellationToken ct = default
    )
    {
        // TODO put this all in a validator

        if (!string.IsNullOrEmpty(tenantId) && tenantId.Length > 200)
        {
            logger.LogWarning(
                "QueryReadingsAsync called with invalid tenantId={tenantId} query parameter",
                tenantId
            );

            throw new ArgumentException(
                "Tenant Id of length greater than 200 characters is invalid"
            );
        }

        if (!string.IsNullOrEmpty(deviceId) && deviceId.Length > 200)
        {
            logger.LogWarning(
                "QueryReadingsAsync called with invalid deviceId={deviceId} query parameter",
                deviceId
            );

            throw new ArgumentException(
                "Device Id of length greater than 200 characters is invalid"
            );
        }

        if (!string.IsNullOrEmpty(type) && type.Length > 200)
        {
            logger.LogWarning(
                "QueryReadingsAsync called with invalid type={type} query parameter",
                type
            );

            throw new ArgumentException("Type of length greater than 200 characters is invalid");
        }

        DateTimeOffset? fromParse = null;

        if (!string.IsNullOrEmpty(from))
        {
            try
            {
                fromParse = DateTimeOffset.Parse(from);
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    "QueryReadingsAsync called with invalid from={from} query parameter",
                    from
                );

                throw;
            }
        }

        DateTimeOffset? toParse = null;

        if (!string.IsNullOrEmpty(to))
        {
            try
            {
                toParse = DateTimeOffset.Parse(to);
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    "QueryReadingsAsync called with invalid to={to} query parameter",
                    to
                );

                throw;
            }
        }

        if (pageSize.HasValue && pageSize > maxPageSize)
        {
            logger.LogWarning(
                "QueryReadingsAsync called with invalid pageSize={pageSize} query parameter",
                pageSize
            );

            throw new ArgumentException(
                $"Page size of '{pageSize}' is greater than configured max page size of '{maxPageSize}'"
            );
        }

        if (pageSize.HasValue && pageSize < 1)
        {
            logger.LogWarning(
                "QueryReadingsAsync called with invalid pageSize={pageSize} query parameter",
                pageSize
            );

            throw new ArgumentException($"Page size of '{pageSize}' is invalid");
        }

        if (page.HasValue && page < 1)
        {
            logger.LogWarning(
                "QueryReadingsAsync called with invalid page={page} query parameter",
                page
            );

            throw new ArgumentException($"Page number of '{page}' is invalid");
        }

        TelemetryReadingsPaginationData? response;

        try
        {
            response = await repository
                .QueryAsync(tenantId, deviceId, type, fromParse, toParse, page, pageSize, ct)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "QueryReadingsAsync threw an exception with parameters tenantId={tenantId}, deviceId={deviceId}, type={type}, from={from}, to={to}, page={page}, pageSize={pageSize}",
                tenantId,
                deviceId,
                type,
                from,
                to,
                page,
                pageSize
            );

            throw;
        }

        logger.LogInformation(
            "QueryReadingsAsync queried readings with parameters tenantId={tenantId}, deviceId={deviceId}, type={type}, from={from}, to={to}, page={page}, pageSize={pageSize}",
            tenantId,
            deviceId,
            type,
            from,
            to,
            page,
            pageSize
        );

        return new TelemetryPaginationView
        {
            TelemetryReadings = [.. response.TelemetryReadings.Select(MapToView)],
            PaginationMetadata = response.PaginationMetadata,
        };
    }

    /// <summary>
    /// Creates a map from a TelemetryReading to a TelemetryView.
    /// Computes a BatteryLow status object.
    /// </summary>
    /// <returns>The mapped TelemetryView.</returns>
    /// <param name="r">The TelemetryReading to map.</param>
    private TelemetryView MapToView(TelemetryReading r) =>
        new()
        {
            TenantId = r.TenantId,
            ExternalId = string.IsNullOrEmpty(r.ExternalId) ? null : r.ExternalId,
            DeviceId = r.DeviceId,
            Type = r.Type,
            Unit = r.Unit,
            Value = r.Value,
            Battery = r.Battery,
            BatteryLow = r.Battery <= batteryLowThreshold,
            Signal = r.Signal,
            RecordedAt = r.RecordedAt.ToString(
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'",
                CultureInfo.InvariantCulture
            ),
            CreatedAt = r.CreatedAt.ToString(
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'",
                CultureInfo.InvariantCulture
            ),
        };
}
