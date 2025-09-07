namespace BudgetExperiment.Api.Controllers;

using BudgetExperiment.Api.Dtos;
using BudgetExperiment.Application.PaySchedules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/// <summary>Pay schedules API.</summary>
[ApiController]
[Route("api/v1/payschedules")]
public sealed class PaySchedulesController : ControllerBase
{
    private readonly IPayScheduleService _service;

    /// <summary>Initializes a new instance of the <see cref="PaySchedulesController"/> class.</summary>
    /// <param name="service">Application service.</param>
    public PaySchedulesController(IPayScheduleService service) => this._service = service;

    /// <summary>Create weekly pay schedule.</summary>
    /// <param name="request">Request body.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created result.</returns>
    [HttpPost("weekly")]
    [ProducesResponseType(typeof(BudgetExperiment.Application.PaySchedules.PayScheduleDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateWeekly([FromBody] CreateWeeklyPayScheduleRequest request, CancellationToken ct)
    {
        if (request.Anchor == default)
        {
            return this.ValidationProblem("Anchor is required.");
        }

        var id = await this._service.CreateWeeklyAsync(request.Anchor, ct).ConfigureAwait(false);
        var dto = await this._service.GetAsync(id, ct).ConfigureAwait(false)!;
        return this.Created($"/api/v1/payschedules/{id}", dto);
    }

    /// <summary>Create monthly pay schedule.</summary>
    /// <param name="request">Request body.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created result.</returns>
    [HttpPost("monthly")]
    [ProducesResponseType(typeof(BudgetExperiment.Application.PaySchedules.PayScheduleDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateMonthly([FromBody] CreateMonthlyPayScheduleRequest request, CancellationToken ct)
    {
        if (request.Anchor == default)
        {
            return this.ValidationProblem("Anchor is required.");
        }

        var id = await this._service.CreateMonthlyAsync(request.Anchor, ct).ConfigureAwait(false);
        var dto = await this._service.GetAsync(id, ct).ConfigureAwait(false)!;
        return this.Created($"/api/v1/payschedules/{id}", dto);
    }

    /// <summary>Get a pay schedule by id.</summary>
    /// <param name="id">Identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BudgetExperiment.Application.PaySchedules.PayScheduleDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var dto = await this._service.GetAsync(id, ct).ConfigureAwait(false);
        if (dto is null)
        {
            return this.NotFound();
        }

        return this.Ok(dto);
    }

    /// <summary>Get occurrence dates for a pay schedule.</summary>
    /// <param name="id">Schedule id.</param>
    /// <param name="start">Range start.</param>
    /// <param name="end">Range end.</param>
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

    /// <summary>List pay schedules.</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paged result.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PayScheduleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var (items, total) = await this._service.ListAsync(page, pageSize, ct).ConfigureAwait(false);
        var result = new PagedResult<PayScheduleDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
        };
        this.Response.Headers["X-Pagination-TotalCount"] = total.ToString(System.Globalization.CultureInfo.InvariantCulture);
        return this.Ok(result);
    }
}
