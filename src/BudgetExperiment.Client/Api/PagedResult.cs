namespace BudgetExperiment.Client.Api;

/// <summary>Generic paged result wrapper.</summary>
/// <typeparam name="T">Item type.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>Gets or sets current page number (1-based).</summary>
    public int Page { get; set; }

    /// <summary>Gets or sets page size.</summary>
    public int PageSize { get; set; }

    /// <summary>Gets or sets total item count.</summary>
    public long TotalCount { get; set; }

    /// <summary>Gets or sets items for the page.</summary>
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
}
