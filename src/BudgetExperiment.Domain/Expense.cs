namespace BudgetExperiment.Domain;

/// <summary>
/// Represents a single expense transaction on a specific date.
/// </summary>
public sealed class Expense
{
    private Expense(Guid id, string description, MoneyValue amount, DateOnly date, string? category)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("Expense description cannot be null or empty.");
        }

        if (amount.Amount <= 0)
        {
            throw new DomainException("Expense amount must be greater than zero.");
        }

        this.Id = id;
        this.Description = description.Trim();
        this.Amount = amount;
        this.Date = date;
        this.Category = category?.Trim();
        this.CreatedUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the unique identifier for this expense.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets the description of the expense (e.g., "Groceries at HEB").
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the amount of the expense.
    /// </summary>
    public MoneyValue Amount { get; }

    /// <summary>
    /// Gets the date when the expense occurred.
    /// </summary>
    public DateOnly Date { get; }

    /// <summary>
    /// Gets the optional category for this expense (e.g., "Groceries", "Gas", "Entertainment").
    /// </summary>
    public string? Category { get; }

    /// <summary>
    /// Gets the timestamp when this expense was created.
    /// </summary>
    public DateTime CreatedUtc { get; }

    /// <summary>
    /// Gets the timestamp when this expense was last updated.
    /// </summary>
    public DateTime? UpdatedUtc { get; private set; }

    /// <summary>
    /// Creates a new expense.
    /// </summary>
    /// <param name="description">The description of the expense.</param>
    /// <param name="amount">The amount of the expense.</param>
    /// <param name="date">The date when the expense occurred.</param>
    /// <param name="category">Optional category for the expense.</param>
    /// <returns>A new expense instance.</returns>
    public static Expense Create(string description, MoneyValue amount, DateOnly date, string? category = null)
    {
        return new Expense(Guid.NewGuid(), description, amount, date, category);
    }

    /// <summary>
    /// Updates the expense details.
    /// </summary>
    /// <param name="description">The new description.</param>
    /// <param name="amount">The new amount.</param>
    /// <param name="date">The new date.</param>
    /// <param name="category">The new category.</param>
    public void Update(string description, MoneyValue amount, DateOnly date, string? category = null)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("Expense description cannot be null or empty.");
        }

        if (amount.Amount <= 0)
        {
            throw new DomainException("Expense amount must be greater than zero.");
        }

        this.UpdatedUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the expense as updated.
    /// </summary>
    public void MarkUpdated()
    {
        this.UpdatedUtc = DateTime.UtcNow;
    }
}
