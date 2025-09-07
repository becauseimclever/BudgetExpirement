namespace BudgetExperiment.Api.Dtos;

/// <summary>Generic paged result wrapper.</summary>
/// <typeparam name="T">Item type.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>Gets the items.</summary>
    public required IReadOnlyList<T> Items
    {
        get; init;
    }

    /// <summary>Gets the current page.</summary>
    public required int Page
    {
        get; init;
    }

    /// <summary>Gets the page size.</summary>
    public required int PageSize
    {
        get; init;
    }

    /// <summary>Gets the total item count.</summary>
    public required long TotalCount
    {
        get; init;
    }
}
