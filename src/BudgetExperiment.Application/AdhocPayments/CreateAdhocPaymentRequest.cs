namespace BudgetExperiment.Application.AdhocPayments;

/// <summary>
/// Request to create a new adhoc payment.
/// </summary>
/// <param name="Description">Description of the payment.</param>
/// <param name="Currency">Currency code.</param>
/// <param name="Amount">Amount of the payment.</param>
/// <param name="Date">Date of the payment.</param>
/// <param name="Category">Optional category.</param>
public sealed record CreateAdhocPaymentRequest(
    string Description,
    string Currency,
    decimal Amount,
    DateOnly Date,
    string? Category = null);
