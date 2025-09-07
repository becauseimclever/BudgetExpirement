namespace BudgetExperiment.Domain.Tests;

using BudgetExperiment.Domain;

public class PayScheduleTests
{
    [Fact]
    public void Weekly_Generates_Expected_Dates_In_Range()
    {
        var schedule = PaySchedule.CreateWeekly(new DateOnly(2025, 1, 3), MoneyValue.Create("USD", 1000m)); // anchor Friday
        var rangeStart = new DateOnly(2025, 1, 1);
        var rangeEnd = new DateOnly(2025, 1, 31);

        var dates = schedule.GetOccurrences(rangeStart, rangeEnd).ToList();
        var expected = new[]
        {
            new DateOnly(2025, 1, 3),
            new DateOnly(2025, 1, 10),
            new DateOnly(2025, 1, 17),
            new DateOnly(2025, 1, 24),
            new DateOnly(2025, 1, 31),
        };
        Assert.Equal(expected, dates);
    }

    [Fact]
    public void Monthly_Clamps_To_Last_Day_When_Shorter_Month()
    {
        var schedule = PaySchedule.CreateMonthly(new DateOnly(2025, 1, 31), MoneyValue.Create("USD", 1000m)); // anchor end-of-month style
        var rangeStart = new DateOnly(2025, 1, 1);
        var rangeEnd = new DateOnly(2025, 3, 31);

        var dates = schedule.GetOccurrences(rangeStart, rangeEnd).ToList();
        var expected = new[]
        {
            new DateOnly(2025, 1, 31),
            new DateOnly(2025, 2, 28),
            new DateOnly(2025, 3, 31),
        };
        Assert.Equal(expected, dates);
    }

    [Fact]
    public void Range_Before_Anchor_Yields_Empty()
    {
    var schedule = PaySchedule.CreateWeekly(new DateOnly(2025, 6, 6), MoneyValue.Create("USD", 1000m));
    var dates = schedule.GetOccurrences(new DateOnly(2025, 5, 1), new DateOnly(2025, 5, 31));
    Assert.Empty(dates);
    }
}
