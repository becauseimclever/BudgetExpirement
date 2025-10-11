using BudgetExperiment.Api.Dtos;
using BudgetExperiment.Application.RecurringSchedules;
using BudgetExperiment.Domain;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetExperiment.Api.Controllers;

/// <summary>Recurring schedules API - unified income and expense schedules.</summary>
[ApiController]
[Route("api/v1/recurring-schedules")]
public sealed class RecurringSchedulesController : ControllerBase
{
    private readonly IRecurringScheduleService _service;

    /// <summary>Initializes a new instance of the <see cref="RecurringSchedulesController"/> class.</summary>
    /// <param name="service">Application service.</param>
    public RecurringSchedulesController(IRecurringScheduleService service) => this._service = service;

    // Income schedule endpoint

    /// <summary>Create income schedule with any recurrence pattern.</summary>
    /// <param name="request">Request body.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created result.</returns>
    [HttpPost("income")]
    [ProducesResponseType(typeof(RecurringScheduleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateIncome([FromBody] CreateIncomeScheduleRequest request, CancellationToken ct)
    {
        if (request.Anchor == default)
        {
            return this.ValidationProblem("Anchor is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Currency) || request.Currency.Length != 3)
        {
            return this.ValidationProblem("Currency must be 3 letters.");
        }

        if (request.Amount <= 0m)
        {
            return this.ValidationProblem("Amount must be positive for income.");
        }

        if (request.Recurrence == RecurrencePattern.Custom && (!request.CustomIntervalDays.HasValue || request.CustomIntervalDays.Value < 1))
        {
            return this.ValidationProblem("CustomIntervalDays is required and must be >= 1 for Custom recurrence pattern.");
        }

        var amount = MoneyValue.Create(request.Currency, request.Amount);
        var id = await this._service.CreateIncomeAsync(
            request.Anchor,
            amount,
            request.Recurrence,
            request.CustomIntervalDays,
            request.Name,
            ct).ConfigureAwait(false);
        var dto = await this._service.GetAsync(id, ct).ConfigureAwait(false);
        return this.Created($"/api/v1/recurring-schedules/{id}", dto);
    }

    // Expense schedule endpoint

    /// <summary>Create expense schedule with any recurrence pattern.</summary>
    /// <param name="request">Request body.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created result.</returns>
    [HttpPost("expenses")]
    [ProducesResponseType(typeof(RecurringScheduleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseScheduleRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return this.ValidationProblem("Name required for expenses.");
        }

        if (request.Anchor == default)
        {
            return this.ValidationProblem("Anchor is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Currency) || request.Currency.Length != 3)
        {
            return this.ValidationProblem("Currency must be 3 letters.");
        }

        if (request.Amount <= 0m)
        {
            return this.ValidationProblem("Amount must be positive (will be stored as negative for expenses).");
        }

        if (request.Recurrence == RecurrencePattern.Custom && (!request.CustomIntervalDays.HasValue || request.CustomIntervalDays.Value < 1))
        {
            return this.ValidationProblem("CustomIntervalDays is required and must be >= 1 for Custom recurrence pattern.");
        }

        var amount = MoneyValue.Create(request.Currency, request.Amount);
        var id = await this._service.CreateExpenseAsync(
            request.Name,
            request.Anchor,
            amount,
            request.Recurrence,
            request.CustomIntervalDays,
            ct).ConfigureAwait(false);
        var dto = await this._service.GetAsync(id, ct).ConfigureAwait(false);
        return this.Created($"/api/v1/recurring-schedules/{id}", dto);
    }

    // Query endpoints

    /// <summary>Get schedule by ID.</summary>
    /// <param name="id">Schedule ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Schedule details.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RecurringScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await this._service.GetAsync(id, ct).ConfigureAwait(false);
        return dto != null ? this.Ok(dto) : this.NotFound();
    }

    /// <summary>Get schedule occurrences within date range.</summary>
    /// <param name="id">Schedule ID.</param>
    /// <param name="start">Start date (inclusive).</param>
    /// <param name="end">End date (inclusive).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of occurrence dates.</returns>
    [HttpGet("{id:guid}/occurrences")]
    [ProducesResponseType(typeof(IEnumerable<DateOnly>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOccurrences(Guid id, [FromQuery] DateOnly start, [FromQuery] DateOnly end, CancellationToken ct)
    {
        if (start == default || end == default)
        {
            return this.ValidationProblem("Start and end dates are required.");
        }

        if (end < start)
        {
            return this.ValidationProblem("End date must be >= start date.");
        }

        var schedule = await this._service.GetAsync(id, ct).ConfigureAwait(false);
        if (schedule == null)
        {
            return this.NotFound();
        }

        var occurrences = await this._service.GetOccurrencesAsync(id, start, end, ct).ConfigureAwait(false);
        return this.Ok(occurrences);
    }

    /// <summary>List all schedules (paged).</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paged list of schedules.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<RecurringScheduleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1 || pageSize > 100)
        {
            pageSize = 20;
        }

        var (items, total) = await this._service.ListAsync(page, pageSize, ct).ConfigureAwait(false);
        var response = new PagedResult<RecurringScheduleDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
        };
        return this.Ok(response);
    }

    /// <summary>List income schedules only (paged).</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paged list of income schedules.</returns>
    [HttpGet("income")]
    [ProducesResponseType(typeof(PagedResult<RecurringScheduleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListIncome([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1 || pageSize > 100)
        {
            pageSize = 20;
        }

        var (items, total) = await this._service.ListIncomeSchedulesAsync(page, pageSize, ct).ConfigureAwait(false);
        var response = new PagedResult<RecurringScheduleDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
        };
        return this.Ok(response);
    }

    /// <summary>List expense schedules only (paged).</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paged list of expense schedules.</returns>
    [HttpGet("expenses")]
    [ProducesResponseType(typeof(PagedResult<RecurringScheduleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListExpenses([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1 || pageSize > 100)
        {
            pageSize = 20;
        }

        var (items, total) = await this._service.ListExpenseSchedulesAsync(page, pageSize, ct).ConfigureAwait(false);
        var response = new PagedResult<RecurringScheduleDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
        };
        return this.Ok(response);
    }

    /// <summary>Update a recurring schedule.</summary>
    /// <param name="id">Schedule ID.</param>
    /// <param name="request">Update request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Updated schedule or not found.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(RecurringScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRecurringScheduleRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Currency) || request.Currency.Length != 3)
        {
            return this.ValidationProblem("Currency must be 3 letters.");
        }

        if (request.Amount == 0m)
        {
            return this.ValidationProblem("Amount cannot be zero.");
        }

        if (request.Anchor == default)
        {
            return this.ValidationProblem("Anchor is required.");
        }

        var amount = MoneyValue.Create(request.Currency, request.Amount);
        var updated = await this._service.UpdateAsync(id, request.Name, amount, request.Anchor, request.Recurrence, request.CustomIntervalDays, ct).ConfigureAwait(false);

        if (!updated)
        {
            return this.NotFound();
        }

        var dto = await this._service.GetAsync(id, ct).ConfigureAwait(false);
        return this.Ok(dto);
    }

    /// <summary>Delete a recurring schedule.</summary>
    /// <param name="id">Schedule ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content or not found.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await this._service.DeleteAsync(id, ct).ConfigureAwait(false);
        return deleted ? this.NoContent() : this.NotFound();
    }

    // Autocomplete support endpoint

    /// <summary>Get distinct schedule names for autocomplete.</summary>
    /// <param name="search">Optional search term to filter names (case-insensitive prefix match).</param>
    /// <param name="maxResults">Maximum number of results to return (default: 10, max: 50).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of distinct schedule names.</returns>
    [HttpGet("descriptions")]
    [ProducesResponseType(typeof(DescriptionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDescriptions([FromQuery] string? search, [FromQuery] int maxResults = 10, CancellationToken ct = default)
    {
        if (maxResults < 1 || maxResults > 50)
        {
            return this.ValidationProblem("maxResults must be between 1 and 50.");
        }

        var descriptions = await this._service.GetDistinctDescriptionsAsync(search, maxResults, ct).ConfigureAwait(false);
        return this.Ok(new DescriptionsResponse(descriptions.ToList(), descriptions.Count));
    }
}
