using Budget.Abstractions.Services;
using Budget.Data;
using Budget.Models;
using Microsoft.EntityFrameworkCore;

namespace Budget.Services
{
	public class TransactionService : ITransactionService
	{
		private readonly BudgetContext _context;

		public TransactionService(BudgetContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Creates a new transaction.
		/// </summary>
		/// <param name="transaction">The transaction to create.</param>
		public void CreateTransaction(Transaction transaction)
		{
			// Ensure the TransactionDate is in UTC format for the database
			transaction.TransactionDate = transaction.TransactionDate.ToUniversalTime();

			// Add the transaction to the database
			_context.Transactions.Add(transaction);
			_context.SaveChanges();
		}

		/// <summary>
		/// Retrieves a transaction by its ID.
		/// </summary>
		/// <param name="id">The ID of the transaction.</param>
		/// <returns>The transaction if found; otherwise, null.</returns>
		public Transaction? GetTransactionById(Guid id)
		{
			return _context.Transactions
				.Include(t => t.Account) // Include related Account data
				.FirstOrDefault(t => t.Id == id);
		}

		/// <summary>
		/// Retrieves all transactions.
		/// </summary>
		/// <returns>A collection of all transactions.</returns>
		public IEnumerable<Transaction> GetAllTransactions()
		{
			return _context.Transactions
				.Include(t => t.Account) // Include related Account data
				.ToList();
		}

		/// <summary>
		 /// Retrieves all transactions for a specific account.
		 /// </summary>
		 /// <param name="accountId">The ID of the account.</param>
		 /// <returns>A collection of transactions for the specified account.</returns>
		public IEnumerable<Transaction> GetTransactionsByAccount(Guid accountId)
		{
			return _context.Transactions
				.AsNoTracking() // Ensures no tracking of entities
				.Where(t => t.Account.Id == accountId)
				.Include(t => t.Account) // Include related Account data if needed
				.ToList();
		}

		/// <summary>
		/// Updates an existing transaction.
		/// </summary>
		/// <param name="transaction">The transaction with updated details.</param>
		public void UpdateTransaction(Transaction transaction)
		{
			_context.Transactions.Update(transaction);
			_context.SaveChanges();
		}

		/// <summary>
		/// Deletes a transaction by its ID.
		/// </summary>
		/// <param name="id">The ID of the transaction to delete.</param>
		public void DeleteTransaction(Guid id)
		{
			var transaction = _context.Transactions.FirstOrDefault(t => t.Id == id);
			if (transaction != null)
			{
				_context.Transactions.Remove(transaction);
				_context.SaveChanges();
			}
		}
	}
}