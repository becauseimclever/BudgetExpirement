namespace Budget.Models
{
	public class Transaction
	{
		public Guid Id { get; set; }
		public string Description { get; set; } = string.Empty;
		public decimal Amount { get; set; }
		public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
		public required Account Account { get; set; }
	}
}
