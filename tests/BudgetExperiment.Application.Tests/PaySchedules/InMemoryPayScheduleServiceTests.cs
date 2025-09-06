namespace BudgetExperiment.Application.Tests.PaySchedules;

using BudgetExperiment.Application.PaySchedules;
using BudgetExperiment.Domain;

public sealed class InMemoryPayScheduleServiceTests
{
    [Fact]
    public void Create_And_Retrieve_Weekly()
    {
        var svc = new InMemoryPayScheduleService();
        var anchor = new DateOnly(2025, 1, 3); // Friday
        var id = svc.CreateWeekly(anchor);
        var occurrences = svc.GetOccurrences(id, new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31)).ToList();
        Assert.Contains(anchor, occurrences);
        Assert.All(occurrences, d => Assert.Equal(DayOfWeek.Friday, d.DayOfWeek));
    }

    [Fact]
    public void Missing_Schedule_Throws()
    {
        var svc = new InMemoryPayScheduleService();
        Assert.Throws<DomainException>(() => svc.GetOccurrences(Guid.NewGuid(), new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 2)));
    }
}
