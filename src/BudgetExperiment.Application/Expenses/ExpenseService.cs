namespace BudgetExperiment.Application.Expenses;

using BudgetExperiment.Domain;

/// <summary>
/// Service for managing expenses.
/// </summary>
public sealed class ExpenseService
{
    private readonly IExpenseReadRepository _expenseReadRepository;
    private readonly IExpenseWriteRepository _expenseWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpenseService"/> class.
    /// </summary>
    /// <param name="expenseReadRepository">The expense read repository.</param>
    /// <param name="expenseWriteRepository">The expense write repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public ExpenseService(
        IExpenseReadRepository expenseReadRepository,
        IExpenseWriteRepository expenseWriteRepository,
        IUnitOfWork unitOfWork)
    {
        this._expenseReadRepository = expenseReadRepository;
        this._expenseWriteRepository = expenseWriteRepository;
        this._unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new expense.
    /// </summary>
    /// <param name="request">The expense creation request.</param>
    /// <returns>The created expense.</returns>
    public async Task<ExpenseResponse> CreateAsync(CreateExpenseRequest request)
    {
        var amount = MoneyValue.Create(request.Currency, request.Amount);
        var expense = Expense.Create(request.Description, amount, request.Date, request.Category);

        await this._expenseWriteRepository.AddAsync(expense);
        await this._unitOfWork.SaveChangesAsync();

        return ToResponse(expense);
    }

    /// <summary>
    /// Gets an expense by its ID.
    /// </summary>
    /// <param name="id">The expense ID.</param>
    /// <returns>The expense if found, otherwise null.</returns>
    public async Task<ExpenseResponse?> GetByIdAsync(Guid id)
    {
        var expense = await this._expenseReadRepository.GetByIdAsync(id);
        return expense != null ? ToResponse(expense) : null;
    }

    /// <summary>
    /// Gets all expenses for a specific date.
    /// </summary>
    /// <param name="date">The date to search for.</param>
    /// <returns>A collection of expenses on the specified date.</returns>
    public async Task<IReadOnlyList<ExpenseResponse>> GetByDateAsync(DateOnly date)
    {
        var expenses = await this._expenseReadRepository.GetByDateAsync(date);
        return expenses.Select(ToResponse).ToList();
    }

    /// <summary>
    /// Gets all expenses within a date range.
    /// </summary>
    /// <param name="startDate">The start date (inclusive).</param>
    /// <param name="endDate">The end date (inclusive).</param>
    /// <returns>A collection of expenses within the date range.</returns>
    public async Task<IReadOnlyList<ExpenseResponse>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        var expenses = await this._expenseReadRepository.GetByDateRangeAsync(startDate, endDate);
        return expenses.Select(ToResponse).ToList();
    }

    /// <summary>
    /// Updates an existing expense.
    /// </summary>
    /// <param name="id">The expense ID.</param>
    /// <param name="request">The update request.</param>
    /// <returns>The updated expense if found, otherwise null.</returns>
    public async Task<ExpenseResponse?> UpdateAsync(Guid id, UpdateExpenseRequest request)
    {
        var expense = await this._expenseReadRepository.GetByIdAsync(id);
        if (expense == null)
        {
            return null;
        }

        var amount = MoneyValue.Create(request.Currency, request.Amount);
        expense.Update(request.Description, amount, request.Date, request.Category);

        await this._expenseWriteRepository.UpdateAsync(expense);
        await this._unitOfWork.SaveChangesAsync();

        return ToResponse(expense);
    }

    /// <summary>
    /// Deletes an expense.
    /// </summary>
    /// <param name="id">The expense ID.</param>
    /// <returns>True if the expense was deleted, otherwise false.</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var expense = await this._expenseReadRepository.GetByIdAsync(id);
        if (expense == null)
        {
            return false;
        }

        await this._expenseWriteRepository.DeleteAsync(expense);
        await this._unitOfWork.SaveChangesAsync();

        return true;
    }

    private static ExpenseResponse ToResponse(Expense expense)
    {
        return new ExpenseResponse(
            expense.Id,
            expense.Description,
            expense.Amount.Currency,
            expense.Amount.Amount,
            expense.Date,
            expense.Category,
            expense.CreatedUtc,
            expense.UpdatedUtc);
    }
}
