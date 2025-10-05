namespace BudgetExperiment.Client.Api;

/// <summary>Aggregate dashboard summary payload.</summary>
public sealed class DashboardSummary
{
    /// <summary>Gets or sets total pay schedule count.</summary>
    public long PayScheduleCount
    {
        get; set;
    }

    /// <summary>Gets or sets recent pay schedules.</summary>
    public List<PayScheduleDto> RecentPaySchedules { get; set; } = new();

    /// <summary>Gets or sets total bill schedule count.</summary>
    public long BillScheduleCount
    {
        get; set;
    }

    /// <summary>Gets or sets recent bill schedules.</summary>
    public List<BillScheduleDto> RecentBillSchedules { get; set; } = new();
}
