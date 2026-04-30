namespace TelemetryIngestionCore.Api.Exceptions;

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
    public string TenantId { get; } = tenantId;
    public string ExternalId { get; } = externalId ?? "";
    public Guid? ExistingId { get; } = existingId;

    public DuplicateExternalIdException(
        string tenantId,
        string? externalId,
        Guid? existingId = null
    )
        : this(tenantId, externalId, existingId, "", default) { }
}
