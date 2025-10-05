using BudgetExperiment.Domain;
using BudgetExperiment.Infrastructure.Data.Repositories;
using BudgetExperiment.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetExperiment.Infrastructure;

/// <summary>
/// DI extensions for infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Adds DbContext, repositories and unit of work.</summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">Configuration root.</param>
    /// <returns>Same service collection.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("AppDb");
        if (string.IsNullOrWhiteSpace(cs))
        {
            throw new InvalidOperationException("Connection string 'AppDb' is required but was not found in configuration.");
        }

        services.AddDbContext<BudgetDbContext>(options => options.UseNpgsql(cs));

        services.AddScoped<IRecurringScheduleReadRepository, RecurringScheduleReadRepository>();
        services.AddScoped<IRecurringScheduleWriteRepository, RecurringScheduleWriteRepository>();
        services.AddScoped<IAdhocTransactionReadRepository, AdhocTransactionReadRepository>();
        services.AddScoped<IAdhocTransactionWriteRepository, AdhocTransactionWriteRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<BudgetDbContext>());
        return services;
    }
}
