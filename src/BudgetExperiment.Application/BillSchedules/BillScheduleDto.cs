namespace BudgetExperiment.Application.BillSchedules;

using BudgetExperiment.Domain;

/// <summary>
/// Data transfer object for bill schedule details.
/// </summary>
public sealed record BillScheduleDto(
    Guid Id,
    string Name,
    string Currency,
    decimal Amount,
    DateOnly Anchor,
    BillSchedule.RecurrenceKind Recurrence)
{
    /// <summary>Create DTO from domain entity.</summary>
    /// <param name="entity">Domain entity.</param>
    /// <returns>DTO.</returns>
    public static BillScheduleDto FromEntity(BillSchedule entity) => new(
        entity.Id,
        entity.Name,
        entity.Amount.Currency,
        entity.Amount.Amount,
        entity.Anchor,
        entity.Recurrence);
}
