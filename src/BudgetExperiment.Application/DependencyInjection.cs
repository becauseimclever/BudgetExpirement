namespace BudgetExperiment.Application;

using BudgetExperiment.Application.AdhocPayments;
using BudgetExperiment.Application.AdhocTransactions;
using BudgetExperiment.Application.Expenses;
using BudgetExperiment.Application.RecurringSchedules;

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
        services.AddScoped<ExpenseService>();
        services.AddScoped<AdhocPaymentService>();

        // Add new unified service
        services.AddScoped<IAdhocTransactionService, AdhocTransactionService>();

        return services;
    }
}
