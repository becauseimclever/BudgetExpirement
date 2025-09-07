namespace BudgetExperiment.Client.Api;

/// <summary>Pay schedule data transfer object (client copy).</summary>
public sealed class PayScheduleDto
{
    /// <summary>Gets or sets identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets anchor date.</summary>
    public DateOnly Anchor { get; set; }

    /// <summary>Gets or sets recurrence descriptor (e.g. weekly/monthly).</summary>
    public string Recurrence { get; set; } = string.Empty;

    /// <summary>Gets or sets creation timestamp (UTC).</summary>
    public DateTime CreatedUtc { get; set; }
}
