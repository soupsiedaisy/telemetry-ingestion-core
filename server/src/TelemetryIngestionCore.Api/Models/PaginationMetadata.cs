namespace TelemetryIngestionCore.Api.Models;

/// <summary>
/// Model for information about pagination
/// </summary>
public class PaginationMetadata
{
    /// <summary>
    /// The current page number of the pagination.
    /// </summary>
    public required int PageNumber { get; set; }

    /// <summary>
    /// The total number of pages available.
    /// </summary>
    public required int PageCount { get; set; }

    /// <summary>
    /// The page size of the pagination.
    /// </summary>
    public required int PageSize { get; set; }
}
