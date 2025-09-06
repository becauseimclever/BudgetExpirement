namespace BudgetExperiment.Application.BillSchedules;

using BudgetExperiment.Domain;

/// <inheritdoc />
public sealed class InMemoryBillScheduleService : IBillScheduleService
{
    private readonly Dictionary<Guid, BillSchedule> _store = new();

    /// <inheritdoc />
    public Guid CreateMonthly(string name, MoneyValue amount, DateOnly anchor)
    {
        var schedule = BillSchedule.CreateMonthly(name, amount, anchor);
        var id = Guid.NewGuid();
        this._store[id] = schedule;
        return id;
    }

    /// <inheritdoc />
    public IEnumerable<DateOnly> GetOccurrences(Guid id, DateOnly start, DateOnly end)
    {
        if (!this._store.TryGetValue(id, out var schedule))
        {
            throw new DomainException("Bill schedule not found.");
        }

        return schedule.GetOccurrences(start, end);
    }
}
