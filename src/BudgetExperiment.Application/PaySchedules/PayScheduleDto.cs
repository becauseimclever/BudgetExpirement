namespace BudgetExperiment.Application.PaySchedules;

using BudgetExperiment.Domain;

/// <summary>
/// Data transfer object representing a pay schedule.
/// </summary>
public sealed record PayScheduleDto(
    Guid Id,
    DateOnly Anchor,
    PaySchedule.RecurrenceKind Recurrence,
    DateTime CreatedUtc,
    DateTime? UpdatedUtc)
{
    /// <summary>Create DTO from entity.</summary>
    /// <param name="entity">Entity.</param>
    /// <returns>DTO.</returns>
    public static PayScheduleDto FromEntity(PaySchedule entity) => new(entity.Id, entity.Anchor, entity.Recurrence, entity.CreatedUtc, entity.UpdatedUtc);
}
