using BudgetExperiment.Application.AdhocTransactions;
using BudgetExperiment.Application.RecurringSchedules;
using BudgetExperiment.Domain;

namespace BudgetExperiment.Application.RunningTotals;

/// <summary>
/// Service for calculating running totals with carryover from previous months.
/// </summary>
public sealed class RunningTotalService : IRunningTotalService
{
    private readonly IRecurringScheduleService _recurringScheduleService;
    private readonly IAdhocTransactionService _adhocTransactionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RunningTotalService"/> class.
    /// </summary>
    /// <param name="recurringScheduleService">Recurring schedule service.</param>
    /// <param name="adhocTransactionService">Adhoc transaction service.</param>
    public RunningTotalService(
        IRecurringScheduleService recurringScheduleService,
        IAdhocTransactionService adhocTransactionService)
    {
        this._recurringScheduleService = recurringScheduleService ?? throw new ArgumentNullException(nameof(recurringScheduleService));
        this._adhocTransactionService = adhocTransactionService ?? throw new ArgumentNullException(nameof(adhocTransactionService));
    }

    /// <summary>
    /// Determines if there might be data before the specified month.
    /// This is a simple heuristic to avoid infinite recursion.
    /// </summary>
    /// <param name="monthStart">The month start date.</param>
    /// <returns>True if there might be data before this month.</returns>
    public static bool HasDataBeforeMonth(DateOnly monthStart)
    {
        // Reasonable cutoff to avoid infinite recursion
        // In a real application, you might query for the earliest transaction date
        return monthStart >= new DateOnly(2020, 1, 1);
    }

    /// <inheritdoc />
    public async Task<Dictionary<DateOnly, DailyRunningTotalResponse>> GetRunningTotalsForMonthAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        var monthStart = new DateOnly(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        // Get the CUMULATIVE running total from ALL previous months
        var cumulativeRunningTotal = await this.GetCumulativeRunningTotalAsync(year, month, cancellationToken).ConfigureAwait(false);

        // Get all transactions for the current month
        var dailyAmounts = await this.GetDailyAmountsForMonthAsync(year, month, cancellationToken).ConfigureAwait(false);

        // Calculate running totals for each day starting from the cumulative total
        var result = new Dictionary<DateOnly, DailyRunningTotalResponse>();
        var runningTotal = cumulativeRunningTotal;

        for (var day = monthStart; day <= monthEnd; day = day.AddDays(1))
        {
            var dailyAmount = dailyAmounts.GetValueOrDefault(day, 0m);
            runningTotal += dailyAmount;

            result[day] = new DailyRunningTotalResponse
            {
                Date = day,
                DailyAmount = dailyAmount,
                RunningTotal = runningTotal,
            };
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<decimal> GetEndOfMonthTotalAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        // Get the cumulative total from all previous months
        var cumulativeTotal = await this.GetCumulativeRunningTotalAsync(year, month, cancellationToken).ConfigureAwait(false);

        // Add the current month's transactions
        var currentMonthTotal = await this.CalculateMonthlyTotalAsync(year, month, cancellationToken).ConfigureAwait(false);

        return cumulativeTotal + currentMonthTotal;
    }

    /// <summary>
    /// Gets the running total from the end of the previous month.
    /// This calculates the cumulative total from all previous months.
    /// </summary>
    /// <param name="currentYear">The current year.</param>
    /// <param name="currentMonth">The current month.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cumulative running total from all previous months.</returns>
    private async Task<decimal> GetCumulativeRunningTotalAsync(int currentYear, int currentMonth, CancellationToken cancellationToken)
    {
        // Calculate previous month
        var previousMonth = currentMonth - 1;
        var previousYear = currentYear;

        if (previousMonth < 1)
        {
            previousMonth = 12;
            previousYear = currentYear - 1;
        }

        // Base case: if we've gone back to our starting point (e.g., 2020), return 0
        if (previousYear < 2020)
        {
            return 0m;
        }

        // Recursively get the cumulative total up to the previous month
        var cumulativePreviousTotal = await this.GetCumulativeRunningTotalAsync(previousYear, previousMonth, cancellationToken).ConfigureAwait(false);

        // Get the transactions for the previous month
        var previousMonthTransactions = await this.CalculateMonthlyTotalAsync(previousYear, previousMonth, cancellationToken).ConfigureAwait(false);

        // Return cumulative total plus the previous month's transactions
        return cumulativePreviousTotal + previousMonthTransactions;
    }

    /// <summary>
    /// Gets daily amounts for a specific month by combining recurring schedules and adhoc transactions.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dictionary mapping dates to daily amounts.</returns>
    private async Task<Dictionary<DateOnly, decimal>> GetDailyAmountsForMonthAsync(int year, int month, CancellationToken cancellationToken)
    {
        var monthStart = new DateOnly(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var dailyAmounts = new Dictionary<DateOnly, decimal>();

        // Get adhoc transactions for the month
        var adhocTransactions = await this._adhocTransactionService.GetByDateRangeAsync(monthStart, monthEnd, cancellationToken).ConfigureAwait(false);
        foreach (var transaction in adhocTransactions)
        {
            if (!dailyAmounts.ContainsKey(transaction.Date))
            {
                dailyAmounts[transaction.Date] = 0m;
            }

            dailyAmounts[transaction.Date] += transaction.Amount;
        }

        // Get all recurring schedules and their occurrences for the month
        var (schedules, _) = await this._recurringScheduleService.ListAsync(1, 1000, cancellationToken).ConfigureAwait(false); // Get all schedules

        foreach (var schedule in schedules)
        {
            var occurrences = await this._recurringScheduleService.GetOccurrencesAsync(schedule.Id, monthStart, monthEnd, cancellationToken).ConfigureAwait(false);

            foreach (var occurrence in occurrences)
            {
                if (!dailyAmounts.ContainsKey(occurrence))
                {
                    dailyAmounts[occurrence] = 0m;
                }

                dailyAmounts[occurrence] += schedule.Amount;
            }
        }

        return dailyAmounts;
    }

    /// <summary>
    /// Calculates the total amount for a specific month (transactions only, no carryover).
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The total amount for the month.</returns>
    private async Task<decimal> CalculateMonthlyTotalAsync(int year, int month, CancellationToken cancellationToken)
    {
        // Calculate month totals by getting all transactions for the month
        // This is the NET change for this month only (no carryover)
        var monthStart = new DateOnly(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        decimal monthTotal = 0m;

        // Get adhoc transactions total for the month
        var adhocTransactions = await this._adhocTransactionService.GetByDateRangeAsync(monthStart, monthEnd, cancellationToken).ConfigureAwait(false);
        monthTotal += adhocTransactions.Sum(t => t.Amount);

        // Get recurring schedule occurrences total for the month
        var (schedules, _) = await this._recurringScheduleService.ListAsync(1, 1000, cancellationToken).ConfigureAwait(false);

        foreach (var schedule in schedules)
        {
            var occurrences = await this._recurringScheduleService.GetOccurrencesAsync(schedule.Id, monthStart, monthEnd, cancellationToken).ConfigureAwait(false);
            monthTotal += occurrences.Count() * schedule.Amount;
        }

        return monthTotal;
    }
}
