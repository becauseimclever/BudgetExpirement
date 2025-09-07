namespace BudgetExperiment.Client.Api;

/// <summary>Typed client abstraction for Budget API calls.</summary>
public interface IBudgetApiClient
{
    /// <summary>Gets dashboard summary.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Dashboard summary payload or null if not found.</returns>
    Task<DashboardSummary?> GetDashboardAsync(CancellationToken ct = default);

    /// <summary>Gets a page of pay schedules.</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paged result or null if request fails serialization.</returns>
    Task<PagedResult<PayScheduleDto>?> GetPaySchedulesAsync(int page, int pageSize, CancellationToken ct = default);

    /// <summary>Gets a page of bill schedules.</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paged result or null if request fails serialization.</returns>
    Task<PagedResult<BillScheduleDto>?> GetBillSchedulesAsync(int page, int pageSize, CancellationToken ct = default);
}
