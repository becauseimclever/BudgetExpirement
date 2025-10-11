using BudgetExperiment.Api.Dtos;
using BudgetExperiment.Application.AdhocTransactions;
using BudgetExperiment.Domain;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetExperiment.Api.Controllers;

/// <summary>Adhoc transactions API - unified income and expense transactions.</summary>
[ApiController]
[Route("api/v1/adhoc-transactions")]
public sealed class AdhocTransactionsController : ControllerBase
{
    private readonly IAdhocTransactionService _service;

    /// <summary>Initializes a new instance of the <see cref="AdhocTransactionsController"/> class.</summary>
    /// <param name="service">Application service.</param>
    public AdhocTransactionsController(IAdhocTransactionService service) => this._service = service;

    // Income transaction endpoint

    /// <summary>Create income transaction.</summary>
    /// <param name="request">Request body.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created result.</returns>
    [HttpPost("income")]
    [ProducesResponseType(typeof(AdhocTransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateIncome([FromBody] CreateIncomeTransactionRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Description))
        {
            return this.ValidationProblem("Description is required.");
        }

        if (request.Date == default)
        {
            return this.ValidationProblem("Date is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Currency) || request.Currency.Length != 3)
        {
            return this.ValidationProblem("Currency must be 3 letters.");
        }

        if (request.Amount <= 0m)
        {
            return this.ValidationProblem("Amount must be positive for income.");
        }

        var dto = await this._service.CreateIncomeAsync(request, ct).ConfigureAwait(false);
        return this.Created($"/api/v1/adhoc-transactions/{dto.Id}", dto);
    }

    // Expense transaction endpoint

    /// <summary>Create expense transaction.</summary>
    /// <param name="request">Request body.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created result.</returns>
    [HttpPost("expenses")]
    [ProducesResponseType(typeof(AdhocTransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseTransactionRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Description))
        {
            return this.ValidationProblem("Description is required.");
        }

        if (request.Date == default)
        {
            return this.ValidationProblem("Date is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Currency) || request.Currency.Length != 3)
        {
            return this.ValidationProblem("Currency must be 3 letters.");
        }

        if (request.Amount <= 0m)
        {
            return this.ValidationProblem("Amount must be positive (will be stored as negative for expenses).");
        }

        var dto = await this._service.CreateExpenseAsync(request, ct).ConfigureAwait(false);
        return this.Created($"/api/v1/adhoc-transactions/{dto.Id}", dto);
    }

    // Query endpoints

    /// <summary>Get transaction by ID.</summary>
    /// <param name="id">Transaction ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Transaction details.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AdhocTransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await this._service.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto != null ? this.Ok(dto) : this.NotFound();
    }

    /// <summary>Get transactions for a specific date.</summary>
    /// <param name="date">The date to search for.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of transactions on the specified date.</returns>
    [HttpGet("by-date/{date}")]
    [ProducesResponseType(typeof(IEnumerable<AdhocTransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByDate(DateOnly date, CancellationToken ct)
    {
        if (date == default)
        {
            return this.ValidationProblem("Date is required.");
        }

        var transactions = await this._service.GetByDateAsync(date, ct).ConfigureAwait(false);
        return this.Ok(transactions);
    }

    /// <summary>Get transactions within a date range.</summary>
    /// <param name="startDate">Start date (inclusive).</param>
    /// <param name="endDate">End date (inclusive).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of transactions within the date range.</returns>
    [HttpGet("by-date-range")]
    [ProducesResponseType(typeof(IEnumerable<AdhocTransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate, CancellationToken ct)
    {
        if (startDate == default || endDate == default)
        {
            return this.ValidationProblem("Start and end dates are required.");
        }

        if (endDate < startDate)
        {
            return this.ValidationProblem("End date must be >= start date.");
        }

        var transactions = await this._service.GetByDateRangeAsync(startDate, endDate, ct).ConfigureAwait(false);
        return this.Ok(transactions);
    }

    /// <summary>List all transactions (paged).</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paged list of transactions.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdhocTransactionResponse>), StatusCodes.Status200OK)]
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
        var response = new PagedResult<AdhocTransactionResponse>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
        };
        return this.Ok(response);
    }

    /// <summary>List income transactions only (paged).</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paged list of income transactions.</returns>
    [HttpGet("income")]
    [ProducesResponseType(typeof(PagedResult<AdhocTransactionResponse>), StatusCodes.Status200OK)]
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

        var (items, total) = await this._service.ListIncomeTransactionsAsync(page, pageSize, ct).ConfigureAwait(false);
        var response = new PagedResult<AdhocTransactionResponse>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
        };
        return this.Ok(response);
    }

    /// <summary>List expense transactions only (paged).</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paged list of expense transactions.</returns>
    [HttpGet("expenses")]
    [ProducesResponseType(typeof(PagedResult<AdhocTransactionResponse>), StatusCodes.Status200OK)]
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

        var (items, total) = await this._service.ListExpenseTransactionsAsync(page, pageSize, ct).ConfigureAwait(false);
        var response = new PagedResult<AdhocTransactionResponse>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
        };
        return this.Ok(response);
    }

    /// <summary>Update an adhoc transaction.</summary>
    /// <param name="id">Transaction ID.</param>
    /// <param name="request">Update request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Updated transaction or not found.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AdhocTransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAdhocTransactionRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Description))
        {
            return this.ValidationProblem("Description is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Currency) || request.Currency.Length != 3)
        {
            return this.ValidationProblem("Currency must be 3 letters.");
        }

        if (request.Amount == 0m)
        {
            return this.ValidationProblem("Amount cannot be zero.");
        }

        if (request.Date == default)
        {
            return this.ValidationProblem("Date is required.");
        }

        var dto = await this._service.UpdateAsync(id, request, ct).ConfigureAwait(false);
        return dto != null ? this.Ok(dto) : this.NotFound();
    }

    /// <summary>Delete an adhoc transaction.</summary>
    /// <param name="id">Transaction ID.</param>
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

    /// <summary>Get distinct transaction descriptions for autocomplete.</summary>
    /// <param name="search">Optional search term to filter descriptions (case-insensitive prefix match).</param>
    /// <param name="maxResults">Maximum number of results to return (default: 10, max: 50).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of distinct descriptions.</returns>
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
