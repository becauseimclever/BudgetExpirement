namespace BudgetExperiment.Application.RecurringSchedules;

using BudgetExperiment.Domain;

/// <summary>
/// Data transfer object for recurring schedule details.
/// Unified DTO that replaces both PayScheduleDto and BillScheduleDto.
/// </summary>
public sealed record RecurringScheduleDto(
    Guid Id,
    string? Name,
    string Currency,
    decimal Amount,
    DateOnly Anchor,
    RecurrencePattern Recurrence,
    ScheduleType ScheduleType,
    int? DaysInterval,
    DateTime CreatedUtc,
    DateTime? UpdatedUtc)
{
    /// <summary>Create DTO from domain entity.</summary>
    /// <param name="entity">Domain entity.</param>
    /// <returns>DTO.</returns>
    public static RecurringScheduleDto FromEntity(RecurringSchedule entity) => new(
        entity.Id,
        entity.Name,
        entity.Amount.Currency,
        entity.Amount.Amount,
        entity.Anchor,
        entity.Recurrence,
        entity.ScheduleType,
        entity.DaysInterval,
        entity.CreatedUtc,
        entity.UpdatedUtc);
}
