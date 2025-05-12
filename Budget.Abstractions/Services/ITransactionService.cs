using Budget.Models;

namespace Budget.Abstractions.Services
{
	/// <summary>
	/// Provides methods to manage transactions.
	/// </summary>
	public interface ITransactionService
	{
		/// <summary>
		/// Creates a new transaction.
		/// </summary>
		/// <param name="transaction">The transaction to create.</param>
		void CreateTransaction(Transaction transaction);

		/// <summary>
		/// Retrieves a transaction by its ID.
		/// </summary>
		/// <param name="id">The ID of the transaction.</param>
		/// <returns>The transaction if found; otherwise, null.</returns>
		Transaction? GetTransactionById(Guid id);

		/// <summary>
		/// Retrieves all transactions.
		/// </summary>
		/// <returns>A collection of all transactions.</returns>
		IEnumerable<Transaction> GetAllTransactions();

		/// <summary>
		 /// Retrieves all transactions for a specific account.
		 /// </summary>
		 /// <param name="accountId">The ID of the account.</param>
		 /// <returns>A collection of transactions for the specified account.</returns>
		IEnumerable<Transaction> GetTransactionsByAccount(Guid accountId);

		/// <summary>
		/// Updates an existing transaction.
		/// </summary>
		/// <param name="transaction">The transaction with updated details.</param>
		void UpdateTransaction(Transaction transaction);

		/// <summary>
		/// Deletes a transaction by its ID.
		/// </summary>
		/// <param name="id">The ID of the transaction to delete.</param>
		void DeleteTransaction(Guid id);
	}
}
