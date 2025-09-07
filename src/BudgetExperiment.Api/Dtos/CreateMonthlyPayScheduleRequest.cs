namespace BudgetExperiment.Api.Dtos;

/// <summary>Request to create a monthly pay schedule.</summary>
public sealed class CreateMonthlyPayScheduleRequest
{
    /// <summary>Gets or sets the anchor (first occurrence) date.</summary>
    public DateOnly Anchor
    {
        get; set;
    }
}
