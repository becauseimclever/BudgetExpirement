namespace BudgetExperiment.Client.Api;

using System.Net.Http.Json;

/// <summary>Concrete implementation of <see cref="IBudgetApiClient"/>.</summary>
public sealed class BudgetApiClient : IBudgetApiClient
{
    private readonly HttpClient _http;

    /// <summary>Initializes a new instance of the <see cref="BudgetApiClient"/> class.</summary>
    /// <param name="http">HTTP client instance.</param>
    public BudgetApiClient(HttpClient http) => this._http = http;

    /// <inheritdoc />
    public Task<DashboardSummary?> GetDashboardAsync(CancellationToken ct = default)
        => this._http.GetFromJsonAsync<DashboardSummary>("api/v1/dashboard/summary", ct);

    /// <inheritdoc />
    public Task<PagedResult<PayScheduleDto>?> GetPaySchedulesAsync(int page, int pageSize, CancellationToken ct = default)
        => this._http.GetFromJsonAsync<PagedResult<PayScheduleDto>>($"api/v1/payschedules?page={page}&pageSize={pageSize}", ct);

    /// <inheritdoc />
    public Task<PagedResult<BillScheduleDto>?> GetBillSchedulesAsync(int page, int pageSize, CancellationToken ct = default)
        => this._http.GetFromJsonAsync<PagedResult<BillScheduleDto>>($"api/v1/billschedules?page={page}&pageSize={pageSize}", ct);
}
