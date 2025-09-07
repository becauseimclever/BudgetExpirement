namespace BudgetExperiment.Api.Controllers;

using BudgetExperiment.Api.Dtos;
using BudgetExperiment.Application.BillSchedules;
using BudgetExperiment.Domain;

using Microsoft.AspNetCore.Mvc;

/// <summary>Bill schedules API.</summary>
[ApiController]
[Route("api/v1/billschedules")]
public sealed class BillSchedulesController : ControllerBase
{
    private readonly IBillScheduleService _service;

    /// <summary>Initializes a new instance of the <see cref="BillSchedulesController"/> class.</summary>
    /// <param name="service">Application service.</param>
    public BillSchedulesController(IBillScheduleService service) => this._service = service;

    /// <summary>Create monthly bill schedule.</summary>
    /// <param name="request">Request body.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created result.</returns>
    [HttpPost("monthly")]
    [ProducesResponseType(typeof(BudgetExperiment.Application.BillSchedules.BillScheduleDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateMonthly([FromBody] CreateMonthlyBillScheduleRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return this.ValidationProblem("Name required.");
        }

        if (string.IsNullOrWhiteSpace(request.Currency) || request.Currency.Length != 3)
        {
            return this.ValidationProblem("Currency must be 3 letters.");
        }

        if (request.Amount < 0m)
        {
            return this.ValidationProblem("Amount must be non-negative.");
        }

        if (request.Anchor == default)
        {
            return this.ValidationProblem("Anchor required.");
        }

        var id = await this._service.CreateMonthlyAsync(request.Name.Trim(), MoneyValue.Create(request.Currency.ToUpperInvariant(), request.Amount), request.Anchor, ct).ConfigureAwait(false);
        var dto = await this._service.GetAsync(id, ct).ConfigureAwait(false)!;
        return this.Created($"/api/v1/billschedules/{id}", dto);
    }

    /// <summary>Get bill schedule by id.</summary>
    /// <param name="id">Identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BudgetExperiment.Application.BillSchedules.BillScheduleDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var dto = await this._service.GetAsync(id, ct).ConfigureAwait(false);
        if (dto is null)
        {
            return this.NotFound();
        }

        return this.Ok(dto);
    }

    /// <summary>Get occurrences for bill schedule.</summary>
    /// <param name="id">Identifier.</param>
    /// <param name="start">Start date.</param>
    /// <param name="end">End date.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Occurrences.</returns>
    [HttpGet("{id:guid}/occurrences")]
    [ProducesResponseType(typeof(IEnumerable<DateOnly>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOccurrences(Guid id, [FromQuery] DateOnly start, [FromQuery] DateOnly end, CancellationToken ct)
    {
        if (start == default || end == default)
        {
            return this.ValidationProblem("Start and end are required.");
        }

        var occurrences = await this._service.GetOccurrencesAsync(id, start, end, ct).ConfigureAwait(false);
        return this.Ok(occurrences);
    }
}
