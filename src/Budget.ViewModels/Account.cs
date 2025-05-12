using Budget.Models;

namespace Budget.ViewModels
{
	public class Account
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public AccountType Type { get; set; } = AccountType.Checking;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
	}
}
