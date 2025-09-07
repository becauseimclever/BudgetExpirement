namespace BudgetExperiment.Client.Api;

/// <summary>Pay schedule data transfer object (client copy).</summary>
public sealed class PayScheduleDto
{
    /// <summary>Gets or sets identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets anchor date.</summary>
    public DateOnly Anchor { get; set; }

    /// <summary>Gets or sets recurrence descriptor.</summary>
    public string Recurrence { get; set; } = string.Empty;

    /// <summary>Gets or sets ISO currency code.</summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>Gets or sets monetary amount.</summary>
    public decimal Amount { get; set; }

    /// <summary>Gets or sets custom interval days (null if pattern not custom).</summary>
    public int? DaysInterval { get; set; }

    /// <summary>Gets or sets creation timestamp (UTC).</summary>
    public DateTime CreatedUtc { get; set; }
}
