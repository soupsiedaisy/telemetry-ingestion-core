using System.ComponentModel.DataAnnotations;
using TelemetryIngestionCore.Api.Exceptions;
using TelemetryIngestionCore.Api.Models;
using TelemetryIngestionCore.Api.Services;

namespace TelemetryIngestionCore.Api.Endpoints;

/// <summary>
/// Static class used to map the telemetry endpoints for the application.
/// </summary>
public static class TelemetryEndpoints
{
    /// <summary>
    /// Extends IEndpointRouteBuilder instances with this method to map the telemetry endpoints for the application.
    /// </summary>
    /// <returns>The IEndpointRouteBuilder used for defining route building contracts.</returns>
    /// <param name="builder">The IEndpointRouteBuilder used for defining route building contracts.</param>
    public static IEndpointRouteBuilder MapTelemetryEndpoints(this IEndpointRouteBuilder builder)
    {
        builder
            .MapPost(
                "api/telemetry",
                async (TelemetryDto dto, ITelemetryService service) =>
                {
                    try
                    {
                        var created = await service.CreateReadingAsync(dto);

                        return Results.Created("/api/telemetry", created);
                    }
                    catch (ValidationException ve)
                    {
                        var errors = ve
                            .ValidationResult.MemberNames.DefaultIfEmpty()
                            .Select(m => (Member: m, Error: ve.ValidationResult.ErrorMessage))
                            .GroupBy(t => t.Member)
                            .ToDictionary(
                                g => g.Key ?? string.Empty,
                                g => g.Select(x => x.Error).Where(e => e != null).ToArray()
                            );

                        return Results.ValidationProblem(errors);
                    }
                    catch (ArgumentException ae)
                    {
                        return Results.BadRequest(new { error = ae.Message });
                    }
                    catch (DuplicateExternalIdException dex)
                    {
                        return Results.Conflict(
                            new { error = dex.Message, existingId = dex.ExistingId }
                        );
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem(detail: ex.Message, statusCode: 500);
                    }
                }
            )
            .WithName("CreateTelemetry")
            .Accepts<TelemetryDto>("application/json")
            .Produces<TelemetryView>(statusCode: 201)
            .ProducesValidationProblem()
            .Produces(400)
            .Produces(409)
            .Produces(500);

        builder
            .MapGet(
                "api/telemetry",
                async (
                    ITelemetryService service,
                    string? tenantId,
                    string? deviceId,
                    string? type,
                    string? from,
                    string? to,
                    int? page,
                    int? pageSize,
                    CancellationToken ct
                ) =>
                {
                    try
                    {
                        var results = await service.QueryReadingsAsync(
                            tenantId,
                            deviceId,
                            type,
                            from,
                            to,
                            page,
                            pageSize,
                            ct
                        );
                        return Results.Ok(results);
                    }
                    catch (ArgumentException ae)
                    {
                        return Results.BadRequest(new { error = ae.Message });
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem(detail: ex.Message, statusCode: 500);
                    }
                }
            )
            .WithName("QueryTelemetry")
            .Produces<IEnumerable<TelemetryView>>(statusCode: 200)
            .Produces(400)
            .Produces(500);

        return builder;
    }
}
