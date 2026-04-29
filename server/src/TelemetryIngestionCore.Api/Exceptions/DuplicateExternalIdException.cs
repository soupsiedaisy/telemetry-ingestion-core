namespace TelemetryIngestionCore.Api.Exceptions;

public class DuplicateExternalIdException : Exception
{
    public string TenantId { get; }
    public string? ExternalId { get; }
    public Guid? ExistingId { get; }

    public DuplicateExternalIdException(
        string tenantId,
        string? externalId,
        Guid? existingId = null
    )
        : base($"A reading with ExternalId '{externalId}' already exists for tenant '{tenantId}'.")
    {
        TenantId = tenantId;
        ExternalId = externalId;
        ExistingId = existingId;
    }

    public DuplicateExternalIdException(
        string tenantId,
        string? externalId,
        Guid? existingId,
        Exception inner
    )
        : base(
            $"A reading with ExternalId '{externalId}' already exists for tenant '{tenantId}'.",
            inner
        )
    {
        TenantId = tenantId;
        ExternalId = externalId;
        ExistingId = existingId;
    }
}
