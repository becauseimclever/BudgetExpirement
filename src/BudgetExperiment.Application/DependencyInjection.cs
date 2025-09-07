namespace BudgetExperiment.Application;

using BudgetExperiment.Application.BillSchedules;
using BudgetExperiment.Application.Expenses;
using BudgetExperiment.Application.PaySchedules;
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
        services.AddScoped<IPayScheduleService, PayScheduleService>();
        services.AddScoped<IBillScheduleService, BillScheduleService>();
        services.AddScoped<ExpenseService>();
        return services;
    }
}
