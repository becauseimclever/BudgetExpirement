using System.Net.Http.Json;

using BudgetExperiment.Client.Api;

namespace BudgetExperiment.Client.Services;

/// <summary>
/// Service for managing calendar data caching and calculations.
/// </summary>
public sealed class CalendarDataService
{
    private readonly HttpClient _http;

    // Data caches
    private readonly Dictionary<string, Dictionary<DateOnly, List<RecurringScheduleItem>>> _recurringScheduleOccurrenceCache = new();
    private readonly Dictionary<string, Dictionary<DateOnly, List<AdhocTransactionItem>>> _adhocTransactionCache = new();
    private readonly Dictionary<string, Dictionary<DateOnly, decimal>> _runningTotalsCache = new();
    private List<RecurringScheduleItem> _recurringSchedules = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CalendarDataService"/> class.
    /// </summary>
    /// <param name="http">The HTTP client for API calls.</param>
    public CalendarDataService(HttpClient http)
    {
        this._http = http ?? throw new ArgumentNullException(nameof(http));
    }

    /// <summary>Gets the HttpClient for external API calls.</summary>
    public HttpClient HttpClient => this._http;

    /// <summary>
    /// Formats an amount with proper sign display.
    /// </summary>
    /// <param name="amount">The amount to format.</param>
    /// <returns>Formatted amount string.</returns>
    public static string GetFormattedAmount(decimal amount)
    {
        if (amount == 0)
        {
            return "$0";
        }

        if (amount > 0)
        {
            return $"+${amount:N2}";
        }
        else
        {
            return $"-${Math.Abs(amount):N2}";
        }
    }

    /// <summary>
    /// Loads recurring schedules from the API.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task LoadRecurringSchedulesAsync()
    {
        try
        {
            var result = await this._http.GetFromJsonAsync<PagedResult<RecurringScheduleItem>>("api/v1/recurring-schedules?page=1&pageSize=100");
            if (result?.Items != null)
            {
                this._recurringSchedules = result.Items.ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading recurring schedules: {ex.Message}");
        }
    }

    /// <summary>
    /// Caches recurring schedule occurrences for the specified month.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CacheRecurringScheduleOccurrencesAsync(int year, int month)
    {
        var monthKey = $"{year:D4}-{month:D2}";
        if (this._recurringScheduleOccurrenceCache.ContainsKey(monthKey))
        {
            return;
        }

        var adjustedDate = new DateTime(year, month, 1);
        if (month < 1)
        {
            adjustedDate = new DateTime(year - 1, 12, 1);
            monthKey = $"{year - 1:D4}-12";
        }
        else if (month > 12)
        {
            adjustedDate = new DateTime(year + 1, 1, 1);
            monthKey = $"{year + 1:D4}-01";
        }

        var startDate = DateOnly.FromDateTime(adjustedDate);
        var endDate = DateOnly.FromDateTime(adjustedDate.AddMonths(1).AddDays(-1));

        var monthOccurrences = new Dictionary<DateOnly, List<RecurringScheduleItem>>();

        foreach (var schedule in this._recurringSchedules)
        {
            try
            {
                var occurrences = await this._http.GetFromJsonAsync<List<DateOnly>>($"api/v1/recurring-schedules/{schedule.Id}/occurrences?start={startDate:yyyy-MM-dd}&end={endDate:yyyy-MM-dd}");
                if (occurrences != null)
                {
                    foreach (var occurrence in occurrences)
                    {
                        if (!monthOccurrences.ContainsKey(occurrence))
                        {
                            monthOccurrences[occurrence] = new List<RecurringScheduleItem>();
                        }

                        monthOccurrences[occurrence].Add(schedule);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading occurrences for recurring schedule {schedule.Id}: {ex.Message}");
            }
        }

        this._recurringScheduleOccurrenceCache[monthKey] = monthOccurrences;
    }

    /// <summary>
    /// Caches adhoc transactions for the specified month.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CacheAdhocTransactionsAsync(int year, int month)
    {
        var monthKey = $"{year:D4}-{month:D2}";
        if (this._adhocTransactionCache.ContainsKey(monthKey))
        {
            return;
        }

        var adjustedDate = new DateTime(year, month, 1);
        if (month < 1)
        {
            adjustedDate = new DateTime(year - 1, 12, 1);
            monthKey = $"{year - 1:D4}-12";
        }
        else if (month > 12)
        {
            adjustedDate = new DateTime(year + 1, 1, 1);
            monthKey = $"{year + 1:D4}-01";
        }

        var startDate = DateOnly.FromDateTime(adjustedDate);
        var endDate = DateOnly.FromDateTime(adjustedDate.AddMonths(1).AddDays(-1));

        var monthAdhocTransactions = new Dictionary<DateOnly, List<AdhocTransactionItem>>();

        try
        {
            var transactions = await this._http.GetFromJsonAsync<List<AdhocTransactionItem>>($"api/v1/adhoc-transactions/by-date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            if (transactions != null)
            {
                foreach (var transaction in transactions)
                {
                    if (!monthAdhocTransactions.ContainsKey(transaction.Date))
                    {
                        monthAdhocTransactions[transaction.Date] = new List<AdhocTransactionItem>();
                    }

                    monthAdhocTransactions[transaction.Date].Add(transaction);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading adhoc transactions for {monthKey}: {ex.Message}");
        }

        this._adhocTransactionCache[monthKey] = monthAdhocTransactions;
    }

    /// <summary>
    /// Calculates running totals for the specified month using the API.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CalculateRunningTotalsAsync(int year, int month)
    {
        var monthKey = $"{year:D4}-{month:D2}";

        try
        {
            // Call the API to get running totals with carryover
            var response = await this._http.GetFromJsonAsync<Dictionary<string, DailyRunningTotalResponse>>($"api/v1/running-totals/{year}/{month}");

            if (response != null)
            {
                // Convert string keys back to DateOnly and store in cache
                var monthTotals = new Dictionary<DateOnly, decimal>();

                foreach (var kvp in response)
                {
                    if (DateOnly.TryParse(kvp.Key, out var date))
                    {
                        monthTotals[date] = kvp.Value.RunningTotal;
                    }
                }

                this._runningTotalsCache[monthKey] = monthTotals;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading running totals for {monthKey}: {ex.Message}");

            // Fallback to old calculation method if API fails
            this.CalculateRunningTotalsLocal(year, month);
        }
    }

    /// <summary>
    /// Gets recurring schedules for a specific day.
    /// </summary>
    /// <param name="day">The day to get schedules for.</param>
    /// <param name="monthKey">The month key for caching.</param>
    /// <returns>List of recurring schedules for the day.</returns>
    public List<RecurringScheduleItem> GetRecurringSchedulesForDay(DateOnly day, string monthKey)
    {
        if (!this._recurringScheduleOccurrenceCache.ContainsKey(monthKey))
        {
            return new List<RecurringScheduleItem>();
        }

        var monthOccurrences = this._recurringScheduleOccurrenceCache[monthKey];
        return monthOccurrences.ContainsKey(day) ? monthOccurrences[day] : new List<RecurringScheduleItem>();
    }

    /// <summary>
    /// Gets adhoc transactions for a specific day.
    /// </summary>
    /// <param name="day">The day to get transactions for.</param>
    /// <param name="monthKey">The month key for caching.</param>
    /// <returns>List of adhoc transactions for the day.</returns>
    public List<AdhocTransactionItem> GetAdhocTransactionsForDay(DateOnly day, string monthKey)
    {
        if (!this._adhocTransactionCache.ContainsKey(monthKey))
        {
            return new List<AdhocTransactionItem>();
        }

        var monthTransactions = this._adhocTransactionCache[monthKey];
        return monthTransactions.ContainsKey(day) ? monthTransactions[day] : new List<AdhocTransactionItem>();
    }

    /// <summary>
    /// Gets the running total for a specific day.
    /// </summary>
    /// <param name="day">The day to get running total for.</param>
    /// <param name="monthKey">The month key for caching.</param>
    /// <returns>The running total for the day.</returns>
    public decimal GetRunningTotalForDay(DateOnly day, string monthKey)
    {
        if (this._runningTotalsCache.ContainsKey(monthKey) &&
            this._runningTotalsCache[monthKey].ContainsKey(day))
        {
            return this._runningTotalsCache[monthKey][day];
        }

        return 0m;
    }

    /// <summary>
    /// Gets the daily amount for a specific day.
    /// </summary>
    /// <param name="day">The day to get daily amount for.</param>
    /// <param name="monthKey">The month key for caching.</param>
    /// <returns>The daily amount for the day.</returns>
    public decimal GetDailyAmountForDay(DateOnly day, string monthKey)
    {
        var recurring = this.GetRecurringSchedulesForDay(day, monthKey);
        var adhoc = this.GetAdhocTransactionsForDay(day, monthKey);

        var recurringTotal = recurring.Sum(s => s.Amount);
        var adhocTotal = adhoc.Sum(t => t.Amount);

        return recurringTotal + adhocTotal;
    }

    /// <summary>
    /// Gets the running total tooltip for a specific day.
    /// </summary>
    /// <param name="day">The day to get tooltip for.</param>
    /// <param name="runningTotal">The running total value.</param>
    /// <param name="monthKey">The month key for caching.</param>
    /// <returns>The tooltip text.</returns>
    public string GetRunningTotalTooltip(DateOnly day, decimal runningTotal, string monthKey)
    {
        var dailyAmount = this.GetDailyAmountForDay(day, monthKey);

        var dailyText = dailyAmount == 0
            ? "No transactions"
            : $"Daily total: {GetFormattedAmount(dailyAmount)}";

        return $"{dailyText}\nRunning total through {day:MMM d}: {GetFormattedAmount(runningTotal)}";
    }

    /// <summary>
    /// Clears all cached data.
    /// </summary>
    public void ClearCache()
    {
        this._recurringScheduleOccurrenceCache.Clear();
        this._adhocTransactionCache.Clear();
        this._runningTotalsCache.Clear();
    }

    /// <summary>
    /// Clears cached data for a specific month.
    /// </summary>
    /// <param name="monthKey">The month key to clear cache for.</param>
    public void ClearCacheForMonth(string monthKey)
    {
        this._recurringScheduleOccurrenceCache.Remove(monthKey);
        this._adhocTransactionCache.Remove(monthKey);
        this._runningTotalsCache.Remove(monthKey);
    }

    /// <summary>
    /// Local fallback calculation method (without carryover).
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    private void CalculateRunningTotalsLocal(int year, int month)
    {
        var monthKey = $"{year:D4}-{month:D2}";

        if (!this._runningTotalsCache.ContainsKey(monthKey))
        {
            this._runningTotalsCache[monthKey] = new Dictionary<DateOnly, decimal>();
        }

        var monthTotals = this._runningTotalsCache[monthKey];
        monthTotals.Clear();

        decimal runningTotal = 0m;

        var monthStart = new DateOnly(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        for (var day = monthStart; day <= monthEnd; day = day.AddDays(1))
        {
            var dailyAmount = this.GetDailyAmountForDay(day, monthKey);
            runningTotal += dailyAmount;
            monthTotals[day] = runningTotal;
        }
    }
}
