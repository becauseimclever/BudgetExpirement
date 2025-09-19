namespace BudgetExperiment.Application.AdhocTransactions;

using BudgetExperiment.Domain;

/// <summary>
/// Application service interface for managing adhoc transactions (both income and expenses).
/// Replaces both IExpenseService and IAdhocPaymentService.
/// </summary>
public interface IAdhocTransactionService
{
    // Income transaction creation methods

    /// <summary>Create an income transaction.</summary>
    /// <param name="request">Request body.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created transaction response.</returns>
    Task<AdhocTransactionResponse> CreateIncomeAsync(CreateIncomeTransactionRequest request, CancellationToken cancellationToken = default);

    // Expense transaction creation methods

    /// <summary>Create an expense transaction.</summary>
    /// <param name="request">Request body.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created transaction response.</returns>
    Task<AdhocTransactionResponse> CreateExpenseAsync(CreateExpenseTransactionRequest request, CancellationToken cancellationToken = default);

    // Query methods

    /// <summary>Get transaction details.</summary>
    /// <param name="id">Transaction id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>DTO or null.</returns>
    Task<AdhocTransactionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Get transactions for a specific date.</summary>
    /// <param name="date">The date to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of transactions on the specified date.</returns>
    Task<IReadOnlyList<AdhocTransactionResponse>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default);

    /// <summary>Get transactions within a date range.</summary>
    /// <param name="startDate">Start date (inclusive).</param>
    /// <param name="endDate">End date (inclusive).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of transactions in the date range.</returns>
    Task<IReadOnlyList<AdhocTransactionResponse>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

    /// <summary>List all transactions (paged).</summary>
    /// <param name="page">1-based page.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Items + total count.</returns>
    Task<(IReadOnlyList<AdhocTransactionResponse> Items, int Total)> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>List income transactions only (paged).</summary>
    /// <param name="page">1-based page.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Items + total count.</returns>
    Task<(IReadOnlyList<AdhocTransactionResponse> Items, int Total)> ListIncomeTransactionsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>List expense transactions only (paged).</summary>
    /// <param name="page">1-based page.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Items + total count.</returns>
    Task<(IReadOnlyList<AdhocTransactionResponse> Items, int Total)> ListExpenseTransactionsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    // Update and delete methods

    /// <summary>Update an adhoc transaction.</summary>
    /// <param name="id">Transaction id.</param>
    /// <param name="request">Update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated transaction response, or null if not found.</returns>
    Task<AdhocTransactionResponse?> UpdateAsync(Guid id, UpdateAdhocTransactionRequest request, CancellationToken cancellationToken = default);

    /// <summary>Delete an adhoc transaction.</summary>
    /// <param name="id">Transaction id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
