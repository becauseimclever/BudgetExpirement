namespace BudgetExperiment.Api.Controllers;

using BudgetExperiment.Application.BillSchedules;
using BudgetExperiment.Application.PaySchedules;

using Microsoft.AspNetCore.Mvc;

/// <summary>Dashboard summary endpoints.</summary>
[ApiController]
[Route("api/v1/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IPayScheduleService _paySchedules;
    private readonly IBillScheduleService _billSchedules;

    /// <summary>Initializes a new instance of the <see cref="DashboardController"/> class.</summary>
    /// <param name="paySchedules">Pay schedule service.</param>
    /// <param name="billSchedules">Bill schedule service.</param>
    public DashboardController(IPayScheduleService paySchedules, IBillScheduleService billSchedules)
    {
        this._paySchedules = paySchedules;
        this._billSchedules = billSchedules;
    }

    /// <summary>Gets high-level dashboard data (recent lists + counts).</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Summary object.</returns>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken ct)
    {
        var (payItems, payTotal) = await this._paySchedules.ListAsync(1, 5, ct).ConfigureAwait(false);
        var (billItems, billTotal) = await this._billSchedules.ListAsync(1, 5, ct).ConfigureAwait(false);

        var summary = new
        {
            PayScheduleCount = payTotal,
            RecentPaySchedules = payItems.Select(p => new
            {
                p.Id,
                p.Anchor,
                Recurrence = p.Recurrence.ToString(),
                p.CreatedUtc,
            }),
            BillScheduleCount = billTotal,
            RecentBillSchedules = billItems.Select(b => new
            {
                b.Id,
                b.Name,
                b.Currency,
                b.Amount,
                b.Anchor,
                Recurrence = b.Recurrence.ToString(),
                b.CreatedUtc,
            }),
        };
        return this.Ok(summary);
    }
}
