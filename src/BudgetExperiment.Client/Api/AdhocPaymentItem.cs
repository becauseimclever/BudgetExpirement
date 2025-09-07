namespace BudgetExperiment.Client.Api;

/// <summary>
/// Adhoc payment data from API.
/// </summary>
/// <param name="Id">The unique identifier.</param>
/// <param name="Description">Description of the payment.</param>
/// <param name="Currency">Currency code.</param>
/// <param name="Amount">Amount of the payment.</param>
/// <param name="Date">Date of the payment.</param>
/// <param name="Category">Optional category.</param>
/// <param name="CreatedUtc">When this record was created.</param>
/// <param name="UpdatedUtc">When this record was last updated.</param>
public sealed record AdhocPaymentItem(
    Guid Id,
    string Description,
    string Currency,
    decimal Amount,
    DateOnly Date,
    string? Category,
    DateTime CreatedUtc,
    DateTime? UpdatedUtc);
