using Budget.Models;

namespace Budget.Abstractions.Services
{
	public interface IAccountService
	{
		/// <summary>
		/// Creates a new account.
		/// </summary>
		/// <param name="account">The account to create.</param>
		void CreateAccount(Account account);
		/// <summary>
		/// Retrieves an account by its ID.
		/// </summary>
		/// <param name="id">The ID of the account.</param>
		Account GetAccountById(Guid id);
		/// <summary>
		 /// Retrieves all accounts.
		 /// </summary>
		 /// <returns>A collection of all accounts.</returns>
		IEnumerable<Account> GetAllAccounts();
		/// <summary>
		/// Updates an existing account.
		/// </summary>
		/// <param name="account">The account to update.</param>
		void UpdateAccount(Account account);
		/// <summary>
		/// Deletes an account by its ID.
		/// </summary>
		/// <param name="id">The ID of the account.</param>
		void DeleteAccount(Guid id);

		/// <summary>
		/// Adds a transaction to an account.
		/// </summary>
		/// <param name="accountId">The ID of the account.</param>
		/// <param name="transaction">The transaction to add.</param>
		void AddTransaction(Guid accountId, Transaction transaction);

		/// <summary>
		/// Updates a transaction associated with an account.
		/// </summary>
		/// <param name="transaction">The transaction to update.</param>
		void UpdateTransaction(Transaction transaction);

		/// <summary>
		/// Deletes a transaction by its ID.
		/// </summary>
		/// <param name="transactionId">The ID of the transaction to delete.</param>
		void DeleteTransaction(Guid transactionId);
	}
}
