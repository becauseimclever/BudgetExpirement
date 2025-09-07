namespace BudgetExperiment.Api.Dtos;

/// <summary>Request to create a weekly pay schedule.</summary>
public sealed class CreateWeeklyPayScheduleRequest
{
    /// <summary>Gets or sets the anchor (first occurrence) date.</summary>
    public DateOnly Anchor
    {
        get; set;
    }
}
