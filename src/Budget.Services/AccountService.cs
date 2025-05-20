using Budget.Abstractions.Services;
using Budget.Data;
using Budget.Models;
using Microsoft.EntityFrameworkCore;

namespace Budget.Services
{
	public class AccountService : IAccountService
	{
		private readonly BudgetContext context;

		public AccountService(BudgetContext context)
		{
			this.context = context;
		}

		public void CreateAccount(Account account)
		{
			account.Id = Guid.NewGuid();
			account.CreatedAt = DateTime.UtcNow;
			account.UpdatedAt = DateTime.UtcNow;

			context.Accounts.Add(account);
			context.SaveChanges();
		}

		public Account GetAccountById(Guid id)
		{
			var account = context.Accounts
				.AsNoTracking() // Ensures no tracking of entities
				.Where(a => a.Id == id)
				.Select(a => new Account
				{
					Id = a.Id,
					Name = a.Name,
					Type = a.Type,
					CreatedAt = a.CreatedAt,
					UpdatedAt = a.UpdatedAt
					// Exclude Transactions
				})
				.FirstOrDefault();

			if (account == null)
			{
				throw new KeyNotFoundException("Account not found.");
			}

			return account;
		}

		public IEnumerable<Account> GetAllAccounts()
		{
			return context.Accounts.ToList();
		}

		public void UpdateAccount(Account account)
		{
			var existingAccount = context.Accounts.Find(account.Id);
			if (existingAccount == null)
			{
				throw new KeyNotFoundException("Account not found.");
			}

			existingAccount.Name = account.Name;
			existingAccount.Type = account.Type;
			existingAccount.UpdatedAt = DateTime.UtcNow;

			context.SaveChanges();
		}

		public void DeleteAccount(Guid id)
		{
			var account = context.Accounts.Find(id);
			if (account == null)
			{
				throw new KeyNotFoundException("Account not found.");
			}

			context.Accounts.Remove(account);
			context.SaveChanges();
		}

		public void AddTransaction(Guid accountId, Transaction transaction)
		{
			var account = context.Accounts.Find(accountId);
			if (account == null)
			{
				throw new KeyNotFoundException("Account not found.");
			}

			transaction.Id = Guid.NewGuid();
			transaction.TransactionDate = transaction.TransactionDate.ToUniversalTime();
			transaction.Account = account;

			context.Transactions.Add(transaction);
			context.SaveChanges();
		}

		public void UpdateTransaction(Transaction transaction)
		{
			var existingTransaction = context.Transactions.Find(transaction.Id);
			if (existingTransaction == null)
			{
				throw new KeyNotFoundException("Transaction not found.");
			}

			existingTransaction.Description = transaction.Description;
			existingTransaction.Amount = transaction.Amount;
			existingTransaction.TransactionDate = transaction.TransactionDate;

			context.SaveChanges();
		}

		public void DeleteTransaction(Guid transactionId)
		{
			var transaction = context.Transactions.Find(transactionId);
			if (transaction == null)
			{
				throw new KeyNotFoundException("Transaction not found.");
			}

			context.Transactions.Remove(transaction);
			context.SaveChanges();
		}

		public void AddTransactionsInBulk(Guid accountId, IEnumerable<Transaction> transactions)
		{
			var account = context.Accounts.Find(accountId);
			if (account == null)
			{
				throw new KeyNotFoundException("Account not found.");
			}

			foreach (var transaction in transactions)
			{
				transaction.Id = Guid.NewGuid();
				transaction.TransactionDate = transaction.TransactionDate.ToUniversalTime();
				transaction.Account = account;
			}

			context.Transactions.AddRange(transactions);
			context.SaveChanges();
		}
	}
}
