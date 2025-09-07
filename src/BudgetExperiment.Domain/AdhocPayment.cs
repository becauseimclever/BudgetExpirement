namespace BudgetExperiment.Domain;

/// <summary>
/// Represents an ad-hoc payment like bonuses, gifts, or other irregular income.
/// </summary>
public sealed class AdhocPayment
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdhocPayment"/> class.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="description">Description of the payment.</param>
    /// <param name="money">The money value.</param>
    /// <param name="date">The date of the payment.</param>
    /// <param name="category">Optional category.</param>
    /// <param name="createdUtc">When this record was created.</param>
    /// <param name="updatedUtc">When this record was last updated.</param>
    public AdhocPayment(
        Guid id,
        string description,
        MoneyValue money,
        DateOnly date,
        string? category = null,
        DateTime createdUtc = default,
        DateTime? updatedUtc = null)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be null or empty.", nameof(description));
        }

        this.Id = id;
        this.Description = description.Trim();
        this.Money = money ?? throw new ArgumentNullException(nameof(money));
        this.Date = date;
        this.Category = string.IsNullOrWhiteSpace(category) ? null : category.Trim();
        this.CreatedUtc = createdUtc == default ? DateTime.UtcNow : createdUtc;
        this.UpdatedUtc = updatedUtc;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdhocPayment"/> class.
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private AdhocPayment()
    {
        this.Id = Guid.Empty;
        this.Description = string.Empty;
        this.Money = MoneyValue.Zero("USD");
        this.Date = DateOnly.MinValue;
        this.CreatedUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets the description of the payment.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the money value.
    /// </summary>
    public MoneyValue Money { get; }

    /// <summary>
    /// Gets the date of the payment.
    /// </summary>
    public DateOnly Date { get; }

    /// <summary>
    /// Gets the optional category.
    /// </summary>
    public string? Category { get; }

    /// <summary>
    /// Gets when this record was created in UTC.
    /// </summary>
    public DateTime CreatedUtc { get; }

    /// <summary>
    /// Gets when this record was last updated in UTC.
    /// </summary>
    public DateTime? UpdatedUtc { get; private set; }

    /// <summary>
    /// Creates a new AdhocPayment with default ID and creation timestamp.
    /// </summary>
    /// <param name="description">Description of the payment.</param>
    /// <param name="money">The money value.</param>
    /// <param name="date">The date of the payment.</param>
    /// <param name="category">Optional category.</param>
    /// <returns>A new AdhocPayment instance.</returns>
    public static AdhocPayment Create(string description, MoneyValue money, DateOnly date, string? category = null)
    {
        return new AdhocPayment(Guid.NewGuid(), description, money, date, category);
    }

    /// <summary>
    /// Marks the adhoc payment as updated.
    /// </summary>
    public void MarkUpdated()
    {
        this.UpdatedUtc = DateTime.UtcNow;
    }
}
