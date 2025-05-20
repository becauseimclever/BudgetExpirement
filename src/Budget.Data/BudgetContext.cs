using Budget.Models;
using Microsoft.EntityFrameworkCore;

namespace Budget.Data
{
	public class BudgetContext : DbContext
	{
		public DbSet<Account> Accounts { get; set; } = null!;
		public DbSet<Transaction> Transactions { get; set; } = null!;

		public BudgetContext(DbContextOptions<BudgetContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Configure the one-to-many relationship between Account and Transaction
			modelBuilder.Entity<Transaction>()
				.HasOne(t => t.Account)
				.WithMany(a => a.Transactions)
				.HasForeignKey("AccountId") // Foreign key property
				.IsRequired();

			base.OnModelCreating(modelBuilder);
		}
	}
}
