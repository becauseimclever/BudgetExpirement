namespace BudgetExperiment.Application.Tests.BillSchedules;

using BudgetExperiment.Application.BillSchedules;
using BudgetExperiment.Domain;

public sealed class InMemoryBillScheduleServiceTests
{
    [Fact]
    public void Create_And_Retrieve_Monthly()
    {
        var svc = new InMemoryBillScheduleService();
        var id = svc.CreateMonthly("Rent", MoneyValue.Create("USD", 1200m), new DateOnly(2025, 1, 5));
        var dates = svc.GetOccurrences(id, new DateOnly(2025, 1, 1), new DateOnly(2025, 3, 31)).ToList();
        Assert.Equal(new[] { new DateOnly(2025, 1, 5), new DateOnly(2025, 2, 5), new DateOnly(2025, 3, 5) }, dates);
    }

    [Fact]
    public void Missing_BillSchedule_Throws()
    {
        var svc = new InMemoryBillScheduleService();
        Assert.Throws<DomainException>(() => svc.GetOccurrences(Guid.NewGuid(), new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31)));
    }
}
