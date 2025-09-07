namespace BudgetExperiment.Api.Dtos;

/// <summary>Common pagination query parameters (placeholder for future list endpoints).</summary>
public sealed class PaginationQuery
{
    private const int MaxPageSize = 200;
    private int _pageSize = 20;

    /// <summary>Gets or sets the 1-based page number.</summary>
    public int Page { get; set; } = 1;

    /// <summary>Gets or sets the page size (capped).</summary>
    public int PageSize
    {
        get => this._pageSize;
        set => this._pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}
