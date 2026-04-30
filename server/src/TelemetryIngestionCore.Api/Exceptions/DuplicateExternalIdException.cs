namespace TelemetryIngestionCore.Api.Exceptions;

/// <summary>
/// Exception thrown when attempting to create a telemetry reading that would duplicate an existing ExternalId for the same TenantId.
/// </summary>
public class DuplicateExternalIdException(
    string tenantId,
    string? externalId,
    Guid? existingId,
    string? message,
    Exception? innerException
)
    : Exception(
        $"A reading with ExternalId '{externalId}' already exists for tenant '{tenantId}'.\n{message}",
        innerException
    )
{
    /// <summary>
    /// Tenant identifier for the detected duplicate.
    /// </summary>
    public string TenantId { get; } = tenantId;

    /// <summary>
    /// The external identifier that caused the conflict. Empty string when null.
    /// </summary>
    /// <remarks>
    /// Though this field is nullable in the database model, this should never be empty or null due to the nature of the conflict.
    /// </remarks>
    public string ExternalId { get; } = externalId ?? "";

    /// <summary>
    /// Identifier of the reading that caused the conflict, if known.
    /// </summary>
    public Guid? ExistingId { get; } = existingId;

    public DuplicateExternalIdException(
        string tenantId,
        string? externalId,
        Guid? existingId = null
    )
        : this(tenantId, externalId, existingId, "", default) { }
}
