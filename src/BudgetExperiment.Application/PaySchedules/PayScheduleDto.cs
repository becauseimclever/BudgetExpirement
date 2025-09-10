namespace BudgetExperiment.Application.PaySchedules;

using BudgetExperiment.Domain;

/// <summary>
/// Data transfer object representing a pay schedule.
/// </summary>
public sealed record PayScheduleDto(
    Guid Id,
    DateOnly Anchor,
    RecurrencePattern Recurrence,
    string Currency,
    decimal Amount,
    int? DaysInterval,
    DateTime CreatedUtc,
    DateTime? UpdatedUtc)
{
    /// <summary>Create DTO from entity.</summary>
    /// <param name="entity">Entity.</param>
    /// <returns>DTO.</returns>
    public static PayScheduleDto FromEntity(PaySchedule entity) => new(
        entity.Id,
        entity.Anchor,
        entity.Recurrence,
        entity.Amount.Currency,
        entity.Amount.Amount,
        entity.DaysInterval,
        entity.CreatedUtc,
        entity.UpdatedUtc);
}
