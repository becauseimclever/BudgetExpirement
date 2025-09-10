namespace BudgetExperiment.Application.AdhocPayments;

/// <summary>
/// Request to update an existing adhoc payment.
/// </summary>
/// <param name="Description">Description of the payment.</param>
/// <param name="Currency">Currency code.</param>
/// <param name="Amount">Amount of the payment.</param>
/// <param name="Date">Date of the payment.</param>
/// <param name="Category">Optional category.</param>
public sealed record UpdateAdhocPaymentRequest(
    string Description,
    string Currency,
    decimal Amount,
    DateOnly Date,
    string? Category = null);
