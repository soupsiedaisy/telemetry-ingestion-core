using System.ComponentModel.DataAnnotations;
using TelemetryIngestionCore.Api.Exceptions;
using TelemetryIngestionCore.Api.Models;
using TelemetryIngestionCore.Api.Services;

namespace TelemetryIngestionCore.Api.Endpoints;

public static class TelemetryEndpoints {
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
                    DateTimeOffset? fromParse = null;
                    if (
                        !string.IsNullOrEmpty(from) && DateTimeOffset.TryParse(from, out var fromDt)
                    )
                    {
                        fromParse = fromDt;
                    }

                    DateTimeOffset? toParse = null;
                    if (!string.IsNullOrEmpty(to) && DateTimeOffset.TryParse(to, out var toDt))
                    {
                        toParse = toDt;
                    }

                    try
                    {
                        var results = await service.QueryReadingsAsync(
                            tenantId,
                            deviceId,
                            type,
                            fromParse,
                            toParse,
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
