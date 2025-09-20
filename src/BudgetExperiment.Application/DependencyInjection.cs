namespace BudgetExperiment.Application;

using BudgetExperiment.Application.AdhocTransactions;
using BudgetExperiment.Application.RecurringSchedules;
using BudgetExperiment.Application.RunningTotals;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Application layer DI registration.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Adds application services.</summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Same collection for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IRecurringScheduleService, RecurringScheduleService>();
        services.AddScoped<IAdhocTransactionService, AdhocTransactionService>();
        services.AddScoped<IRunningTotalService, RunningTotalService>();

        return services;
    }
}
