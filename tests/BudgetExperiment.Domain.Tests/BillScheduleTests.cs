namespace BudgetExperiment.Domain.Tests;

using BudgetExperiment.Domain;

public class BillScheduleTests
{
    [Fact]
    public void Monthly_Generates_Due_Dates_In_Range()
    {
        var schedule = BillSchedule.CreateMonthly("Rent", MoneyValue.Create("USD", 1500m), new DateOnly(2025, 1, 5));
        var dates = schedule.GetOccurrences(new DateOnly(2025, 1, 1), new DateOnly(2025, 3, 31)).ToList();
        var expected = new[]
        {
            new DateOnly(2025, 1, 5),
            new DateOnly(2025, 2, 5),
            new DateOnly(2025, 3, 5),
        };
        Assert.Equal(expected, dates);
    }

    [Fact]
    public void Monthly_Anchor_31_Clamps_For_Short_Months()
    {
        var schedule = BillSchedule.CreateMonthly("Subscription", MoneyValue.Create("USD", 19.99m), new DateOnly(2025, 1, 31));
        var dates = schedule.GetOccurrences(new DateOnly(2025, 1, 1), new DateOnly(2025, 4, 30)).ToList();
        var expected = new[]
        {
            new DateOnly(2025, 1, 31),
            new DateOnly(2025, 2, 28),
            new DateOnly(2025, 3, 31),
            new DateOnly(2025, 4, 30),
        };
        Assert.Equal(expected, dates);
    }

    [Fact]
    public void Range_Before_Anchor_Returns_Empty()
    {
        var schedule = BillSchedule.CreateMonthly("Gym", MoneyValue.Create("USD", 50m), new DateOnly(2025, 6, 10));
        var dates = schedule.GetOccurrences(new DateOnly(2025, 5, 1), new DateOnly(2025, 5, 31));
        Assert.Empty(dates);
    }

    [Fact]
    public void Negative_Amount_Throws()
    {
        Assert.Throws<DomainException>(() => BillSchedule.CreateMonthly("Bad", MoneyValue.Create("USD", -5m), new DateOnly(2025, 1, 1)));
    }
}
