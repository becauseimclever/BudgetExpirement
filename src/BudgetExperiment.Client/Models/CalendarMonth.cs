namespace BudgetExperiment.Client.Models;

/// <summary>
/// Represents a calendar month with its weeks and days.
/// </summary>
public sealed class CalendarMonth
{
    /// <summary>
    /// Gets or sets the year for this calendar month.
    /// </summary>
    public int Year
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets the month number (1-12) for this calendar month.
    /// </summary>
    public int Month
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets the display name of the month.
    /// </summary>
    public string MonthName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the weeks in this calendar month, each containing 7 days.
    /// </summary>
    public List<List<DateOnly>> Weeks { get; set; } = new();

    /// <summary>
    /// Builds a calendar month with proper week layout.
    /// </summary>
    /// <param name="year">The year for the calendar month.</param>
    /// <param name="month">The month number (1-12) for the calendar month.</param>
    /// <returns>A calendar month with properly laid out weeks and days.</returns>
    public static CalendarMonth Build(int year, int month)
    {
        var firstDay = new DateTime(year, month, 1);
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var startOfWeek = (int)firstDay.DayOfWeek; // Sunday = 0
        var weeks = new List<List<DateOnly>>();
        var currentWeek = new List<DateOnly>();

        // Add days from previous month to fill first week
        var prevMonth = firstDay.AddMonths(-1);
        var daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
        for (int i = startOfWeek - 1; i >= 0; i--)
        {
            currentWeek.Add(new DateOnly(prevMonth.Year, prevMonth.Month, daysInPrevMonth - i));
        }

        // Add days of current month
        for (int day = 1; day <= daysInMonth; day++)
        {
            currentWeek.Add(new DateOnly(year, month, day));
            if (currentWeek.Count == 7)
            {
                weeks.Add(currentWeek);
                currentWeek = new List<DateOnly>();
            }
        }

        // Fill remaining cells with next month days
        if (currentWeek.Count > 0)
        {
            var nextMonth = firstDay.AddMonths(1);
            int nextMonthDay = 1;
            while (currentWeek.Count < 7)
            {
                currentWeek.Add(new DateOnly(nextMonth.Year, nextMonth.Month, nextMonthDay));
                nextMonthDay++;
            }

            weeks.Add(currentWeek);
        }

        // Ensure we have 6 rows for consistent layout
        while (weeks.Count < 6)
        {
            var lastWeek = weeks[^1];
            var lastDay = lastWeek[^1];
            var newWeek = new List<DateOnly>();

            for (int i = 1; i <= 7; i++)
            {
                var nextDay = lastDay.AddDays(i);
                newWeek.Add(nextDay);
            }

            weeks.Add(newWeek);
        }

        return new CalendarMonth
        {
            Year = year,
            Month = month,
            MonthName = firstDay.ToString("MMMM"),
            Weeks = weeks,
        };
    }
}
