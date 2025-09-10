namespace BudgetExperiment.Domain;

/// <summary>
/// Common schedule calculation logic shared between different schedule types.
/// </summary>
public static class ScheduleCalculator
{
    /// <summary>
    /// Calculate occurrence dates for a given recurrence pattern within a date range.
    /// </summary>
    /// <param name="anchor">Starting date for the schedule.</param>
    /// <param name="pattern">Recurrence pattern.</param>
    /// <param name="customIntervalDays">Custom interval in days (only used for Custom pattern).</param>
    /// <param name="start">Start of date range (inclusive).</param>
    /// <param name="end">End of date range (inclusive).</param>
    /// <returns>Sequence of occurrence dates within the range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when custom interval is invalid.</exception>
    /// <exception cref="DomainException">Thrown when custom pattern is used without valid interval.</exception>
    public static IEnumerable<DateOnly> CalculateOccurrences(
        DateOnly anchor,
        RecurrencePattern pattern,
        int? customIntervalDays,
        DateOnly start,
        DateOnly end)
    {
        if (start > end)
        {
            yield break;
        }

        switch (pattern)
        {
            case RecurrencePattern.Weekly:
                foreach (var date in CalculateFixedIntervalOccurrences(anchor, 7, start, end))
                {
                    yield return date;
                }

                break;

            case RecurrencePattern.BiWeekly:
                foreach (var date in CalculateFixedIntervalOccurrences(anchor, 14, start, end))
                {
                    yield return date;
                }

                break;

            case RecurrencePattern.Monthly:
                foreach (var date in CalculateMonthlyOccurrences(anchor, start, end))
                {
                    yield return date;
                }

                break;

            case RecurrencePattern.Quarterly:
                foreach (var date in CalculateMonthlyOccurrences(anchor, start, end, 3))
                {
                    yield return date;
                }

                break;

            case RecurrencePattern.SemiAnnual:
                foreach (var date in CalculateMonthlyOccurrences(anchor, start, end, 6))
                {
                    yield return date;
                }

                break;

            case RecurrencePattern.Annual:
                foreach (var date in CalculateMonthlyOccurrences(anchor, start, end, 12))
                {
                    yield return date;
                }

                break;

            case RecurrencePattern.Custom:
                if (!customIntervalDays.HasValue || customIntervalDays.Value <= 0)
                {
                    throw new DomainException("Custom recurrence pattern requires a valid interval in days.");
                }

                foreach (var date in CalculateFixedIntervalOccurrences(anchor, customIntervalDays.Value, start, end))
                {
                    yield return date;
                }

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(pattern), pattern, "Unsupported recurrence pattern.");
        }
    }

    private static IEnumerable<DateOnly> CalculateFixedIntervalOccurrences(
        DateOnly anchor,
        int intervalDays,
        DateOnly start,
        DateOnly end)
    {
        if (intervalDays <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(intervalDays), "Interval must be positive.");
        }

        var current = anchor;

        // Fast forward to the start of the range
        if (current < start)
        {
            var daysSinceAnchor = start.DayNumber - anchor.DayNumber;
            var intervalsPassed = daysSinceAnchor / intervalDays;
            current = anchor.AddDays(intervalsPassed * intervalDays);

            // Ensure we don't go past the start
            if (current < start)
            {
                current = current.AddDays(intervalDays);
            }
        }

        // Generate occurrences within the range
        while (current <= end)
        {
            yield return current;
            current = current.AddDays(intervalDays);
        }
    }

    private static IEnumerable<DateOnly> CalculateMonthlyOccurrences(
        DateOnly anchor,
        DateOnly start,
        DateOnly end,
        int monthInterval = 1)
    {
        if (monthInterval <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(monthInterval), "Month interval must be positive.");
        }

        var current = anchor;

        // Fast forward to get close to the start of the range
        if (current < start)
        {
            var monthsSinceAnchor = ((start.Year - anchor.Year) * 12) + (start.Month - anchor.Month);
            var intervalsPassed = monthsSinceAnchor / monthInterval;
            current = anchor.AddMonths(intervalsPassed * monthInterval);

            // Ensure we don't go past the start
            while (current < start && current <= end)
            {
                current = current.AddMonths(monthInterval);
            }
        }

        // Generate occurrences within the range
        while (current <= end)
        {
            // Handle end-of-month scenarios (e.g., anchor on Jan 31, but Feb doesn't have 31 days)
            var targetDay = anchor.Day;
            var daysInTargetMonth = DateTime.DaysInMonth(current.Year, current.Month);
            var adjustedDay = Math.Min(targetDay, daysInTargetMonth);

            var occurrence = new DateOnly(current.Year, current.Month, adjustedDay);

            if (occurrence >= start && occurrence <= end)
            {
                yield return occurrence;
            }

            current = current.AddMonths(monthInterval);
        }
    }
}
