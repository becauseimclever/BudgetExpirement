namespace BudgetExperiment.Domain;

/// <summary>
/// Enum to categorize the type of adhoc transaction.
/// </summary>
public enum TransactionType
{
    /// <summary>Income transaction (positive amounts).</summary>
    Income,

    /// <summary>Expense transaction (negative amounts).</summary>
    Expense,
}

/// <summary>
/// Represents a unified adhoc transaction that can handle both income (positive amounts) and expenses (negative amounts).
/// Replaces both Expense and AdhocPayment entities.
/// </summary>
public sealed class AdhocTransaction
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdhocTransaction"/> class.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="description">Description of the transaction.</param>
    /// <param name="money">The money value.</param>
    /// <param name="date">The date of the transaction.</param>
    /// <param name="category">Optional category.</param>
    /// <param name="transactionType">Type of transaction (Income or Expense).</param>
    /// <param name="createdUtc">When this record was created.</param>
    /// <param name="updatedUtc">When this record was last updated.</param>
    private AdhocTransaction(
        Guid id,
        string description,
        MoneyValue money,
        DateOnly date,
        string? category,
        TransactionType transactionType,
        DateTime createdUtc = default,
        DateTime? updatedUtc = null)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("AdhocTransaction description cannot be null or empty.");
        }

        this.Id = id;
        this.Description = description.Trim();
        this.Money = money ?? throw new ArgumentNullException(nameof(money));
        this.Date = date;
        this.Category = string.IsNullOrWhiteSpace(category) ? null : category.Trim();
        this.TransactionType = transactionType;
        this.CreatedUtc = createdUtc == default ? DateTime.UtcNow : createdUtc;
        this.UpdatedUtc = updatedUtc;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdhocTransaction"/> class.
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private AdhocTransaction()
    {
        this.Id = Guid.Empty;
        this.Description = string.Empty;
        this.Money = MoneyValue.Zero("USD");
        this.Date = DateOnly.MinValue;
        this.TransactionType = TransactionType.Income;
        this.CreatedUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the description of the transaction.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets the money value.
    /// Convention: Positive for income, negative for expenses.
    /// </summary>
    public MoneyValue Money { get; private set; }

    /// <summary>
    /// Gets the date of the transaction.
    /// </summary>
    public DateOnly Date { get; private set; }

    /// <summary>
    /// Gets the optional category.
    /// </summary>
    public string? Category { get; private set; }

    /// <summary>
    /// Gets the transaction type for categorization.
    /// </summary>
    public TransactionType TransactionType { get; private set; }

    /// <summary>
    /// Gets when this record was created in UTC.
    /// </summary>
    public DateTime CreatedUtc { get; private set; }

    /// <summary>
    /// Gets when this record was last updated in UTC.
    /// </summary>
    public DateTime? UpdatedUtc { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this is an income transaction.
    /// </summary>
    public bool IsIncome => this.TransactionType == TransactionType.Income;

    /// <summary>
    /// Gets a value indicating whether this is an expense transaction.
    /// </summary>
    public bool IsExpense => this.TransactionType == TransactionType.Expense;

    /// <summary>
    /// Gets the display amount (always positive for display purposes).
    /// </summary>
    public decimal DisplayAmount => Math.Abs(this.Money.Amount);

    /// <summary>
    /// Gets the amount prefix for display (+ for income, - for expenses).
    /// </summary>
    public string AmountPrefix => this.IsIncome ? "+" : "-";

    /// <summary>
    /// Gets the formatted amount with prefix for display.
    /// </summary>
    public string FormattedAmount => $"{this.AmountPrefix}${this.DisplayAmount:N0}";

    /// <summary>
    /// Creates a new income transaction with positive amount.
    /// </summary>
    /// <param name="description">Description of the income.</param>
    /// <param name="money">The money value (will be made positive if negative).</param>
    /// <param name="date">The date of the income.</param>
    /// <param name="category">Optional category.</param>
    /// <returns>A new AdhocTransaction instance for income.</returns>
    public static AdhocTransaction CreateIncome(string description, MoneyValue money, DateOnly date, string? category = null)
    {
        // Ensure amount is positive for income
        var positiveAmount = money.Amount < 0 ? MoneyValue.Create(money.Currency, Math.Abs(money.Amount)) : money;

        return new AdhocTransaction(Guid.NewGuid(), description, positiveAmount, date, category, TransactionType.Income);
    }

    /// <summary>
    /// Creates a new expense transaction with negative amount.
    /// </summary>
    /// <param name="description">Description of the expense.</param>
    /// <param name="money">The money value (will be made negative if positive).</param>
    /// <param name="date">The date of the expense.</param>
    /// <param name="category">Optional category.</param>
    /// <returns>A new AdhocTransaction instance for expense.</returns>
    public static AdhocTransaction CreateExpense(string description, MoneyValue money, DateOnly date, string? category = null)
    {
        // Ensure amount is negative for expenses
        var negativeAmount = money.Amount > 0 ? MoneyValue.Create(money.Currency, -money.Amount) : money;

        return new AdhocTransaction(Guid.NewGuid(), description, negativeAmount, date, category, TransactionType.Expense);
    }

    /// <summary>
    /// Updates the adhoc transaction details.
    /// </summary>
    /// <param name="description">The new description.</param>
    /// <param name="money">The new money value.</param>
    /// <param name="date">The new date.</param>
    /// <param name="category">The new category.</param>
    public void Update(string description, MoneyValue money, DateOnly date, string? category = null)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("AdhocTransaction description cannot be null or empty.");
        }

        ArgumentNullException.ThrowIfNull(money);

        this.Description = description.Trim();
        this.Date = date;
        this.Category = string.IsNullOrWhiteSpace(category) ? null : category.Trim();

        // Enforce sign convention based on transaction type
        if (this.TransactionType == TransactionType.Income && money.Amount < 0)
        {
            this.Money = MoneyValue.Create(money.Currency, Math.Abs(money.Amount));
        }
        else if (this.TransactionType == TransactionType.Expense && money.Amount > 0)
        {
            this.Money = MoneyValue.Create(money.Currency, -money.Amount);
        }
        else
        {
            this.Money = money;
        }

        this.UpdatedUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the transaction type and adjusts amount sign accordingly.
    /// </summary>
    /// <param name="newType">New transaction type.</param>
    public void ChangeTransactionType(TransactionType newType)
    {
        this.TransactionType = newType;

        // Adjust amount sign based on new type
        if (newType == TransactionType.Income && this.Money.Amount < 0)
        {
            this.Money = MoneyValue.Create(this.Money.Currency, Math.Abs(this.Money.Amount));
        }
        else if (newType == TransactionType.Expense && this.Money.Amount > 0)
        {
            this.Money = MoneyValue.Create(this.Money.Currency, -this.Money.Amount);
        }

        this.MarkUpdated();
    }

    /// <summary>
    /// Marks the adhoc transaction as updated.
    /// </summary>
    public void MarkUpdated()
    {
        this.UpdatedUtc = DateTime.UtcNow;
    }
}
