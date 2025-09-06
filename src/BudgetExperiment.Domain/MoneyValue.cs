namespace BudgetExperiment.Domain;

/// <summary>
/// Immutable monetary value and currency (2 decimal rounding, away from zero).
/// </summary>
public readonly record struct MoneyValue(string Currency, decimal Amount)
{
    /// <summary>
    /// Adds two monetary values of the same currency.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Summed <see cref="MoneyValue"/>.</returns>
    /// <exception cref="DomainException">Thrown when currencies differ.</exception>
    public static MoneyValue operator +(MoneyValue left, MoneyValue right)
    {
        if (!string.Equals(left.Currency, right.Currency, StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainException("Cannot add amounts with different currencies.");
        }

        return Create(left.Currency, left.Amount + right.Amount);
    }

    /// <summary>
    /// Returns the zero monetary value for the provided currency.
    /// </summary>
    /// <param name="currency">ISO currency code.</param>
    /// <returns>A zero <see cref="MoneyValue"/> for the currency.</returns>
    public static MoneyValue Zero(string currency) => Create(currency, 0m);

    /// <summary>
    /// Factory with validation and normalization.
    /// </summary>
    /// <param name="currency">ISO currency code.</param>
    /// <param name="amount">Amount (>= 0).</param>
    /// <returns>Validated monetary value.</returns>
    /// <exception cref="DomainException">Thrown when invalid.</exception>
    public static MoneyValue Create(string currency, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainException("Currency is required.");
        }

        if (amount < 0m)
        {
            throw new DomainException("Amount cannot be negative.");
        }

        return new MoneyValue(currency.Trim().ToUpperInvariant(), decimal.Round(amount, 2, MidpointRounding.AwayFromZero));
    }

    /// <inheritdoc />
    public override string ToString() => $"{this.Currency} {this.Amount:0.00}";
}
