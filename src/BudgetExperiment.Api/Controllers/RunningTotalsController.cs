using BudgetExperiment.Application.RunningTotals;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetExperiment.Api.Controllers;

/// <summary>Running totals API controller.</summary>
[ApiController]
[Route("api/v1/running-totals")]
public sealed class RunningTotalsController : ControllerBase
{
    private readonly IRunningTotalService _service;

    /// <summary>Initializes a new instance of the <see cref="RunningTotalsController"/> class.</summary>
    /// <param name="service">Running total service.</param>
    public RunningTotalsController(IRunningTotalService service) => this._service = service;

    /// <summary>Get running totals for a specific month with carryover from previous months.</summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Running totals for each day of the month.</returns>
    [HttpGet("{year:int}/{month:int}")]
    [ProducesResponseType(typeof(Dictionary<DateOnly, DailyRunningTotalResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRunningTotalsForMonth(int year, int month, CancellationToken ct)
    {
        if (year < 2020 || year > 2100)
        {
            return this.ValidationProblem("Year must be between 2020 and 2100.");
        }

        if (month < 1 || month > 12)
        {
            return this.ValidationProblem("Month must be between 1 and 12.");
        }

        var runningTotals = await this._service.GetRunningTotalsForMonthAsync(year, month, ct).ConfigureAwait(false);
        return this.Ok(runningTotals);
    }

    /// <summary>Get the running total at the end of a specific month.</summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The running total at the end of the month.</returns>
    [HttpGet("{year:int}/{month:int}/end-total")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetEndOfMonthTotal(int year, int month, CancellationToken ct)
    {
        if (year < 2020 || year > 2100)
        {
            return this.ValidationProblem("Year must be between 2020 and 2100.");
        }

        if (month < 1 || month > 12)
        {
            return this.ValidationProblem("Month must be between 1 and 12.");
        }

        var endTotal = await this._service.GetEndOfMonthTotalAsync(year, month, ct).ConfigureAwait(false);
        return this.Ok(endTotal);
    }
}
