namespace BudgetExperiment.Infrastructure;

using BudgetExperiment.Domain;
using BudgetExperiment.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddScoped<IReadRepository<BillSchedule>, BillScheduleRepository>();
        services.AddScoped<IWriteRepository<BillSchedule>, BillScheduleRepository>();
        services.AddScoped<IReadRepository<PaySchedule>, PayScheduleRepository>();
        services.AddScoped<IWriteRepository<PaySchedule>, PayScheduleRepository>();
        services.AddScoped<IExpenseReadRepository, ExpenseReadRepository>();
        services.AddScoped<IExpenseWriteRepository, ExpenseWriteRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<BudgetDbContext>());
        return services;
    }
}
