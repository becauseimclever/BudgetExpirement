namespace BudgetExperiment.Api.Controllers;

using BudgetExperiment.Application.Expenses;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// API controller for managing expenses.
/// </summary>
[ApiController]
[Route("api/v1/expenses")]
public sealed class ExpensesController : ControllerBase
{
    private readonly ExpenseService _expenseService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpensesController"/> class.
    /// </summary>
    /// <param name="expenseService">The expense service.</param>
    public ExpensesController(ExpenseService expenseService)
    {
        this._expenseService = expenseService;
    }

    /// <summary>
    /// Creates a new expense.
    /// </summary>
    /// <param name="request">The expense creation request.</param>
    /// <returns>The created expense.</returns>
    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> CreateAsync([FromBody] CreateExpenseRequest request)
    {
        var expense = await this._expenseService.CreateAsync(request);
        return this.CreatedAtAction(nameof(this.GetByIdAsync), new { id = expense.Id }, expense);
    }

    /// <summary>
    /// Gets an expense by ID.
    /// </summary>
    /// <param name="id">The expense ID.</param>
    /// <returns>The expense if found.</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExpenseResponse>> GetByIdAsync(Guid id)
    {
        var expense = await this._expenseService.GetByIdAsync(id);
        return expense != null ? this.Ok(expense) : this.NotFound();
    }

    /// <summary>
    /// Gets all expenses for a specific date.
    /// </summary>
    /// <param name="date">The date to search for (YYYY-MM-DD format).</param>
    /// <returns>A collection of expenses on the specified date.</returns>
    [HttpGet("by-date/{date}")]
    public async Task<ActionResult<IReadOnlyList<ExpenseResponse>>> GetByDateAsync(DateOnly date)
    {
        var expenses = await this._expenseService.GetByDateAsync(date);
        return this.Ok(expenses);
    }

    /// <summary>
    /// Gets all expenses within a date range.
    /// </summary>
    /// <param name="startDate">The start date (YYYY-MM-DD format).</param>
    /// <param name="endDate">The end date (YYYY-MM-DD format).</param>
    /// <returns>A collection of expenses within the date range.</returns>
    [HttpGet("by-date-range")]
    public async Task<ActionResult<IReadOnlyList<ExpenseResponse>>> GetByDateRangeAsync(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate)
    {
        var expenses = await this._expenseService.GetByDateRangeAsync(startDate, endDate);
        return this.Ok(expenses);
    }

    /// <summary>
    /// Updates an existing expense.
    /// </summary>
    /// <param name="id">The expense ID.</param>
    /// <param name="request">The update request.</param>
    /// <returns>The updated expense if found.</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ExpenseResponse>> UpdateAsync(Guid id, [FromBody] UpdateExpenseRequest request)
    {
        var expense = await this._expenseService.UpdateAsync(id, request);
        return expense != null ? this.Ok(expense) : this.NotFound();
    }

    /// <summary>
    /// Deletes an expense.
    /// </summary>
    /// <param name="id">The expense ID.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var deleted = await this._expenseService.DeleteAsync(id);
        return deleted ? this.NoContent() : this.NotFound();
    }
}
