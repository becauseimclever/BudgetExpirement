namespace BudgetExperiment.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Design-time factory so dotnet-ef can create the DbContext for migrations.
/// </summary>
public sealed class DesignTimeBudgetDbContextFactory : IDesignTimeDbContextFactory<BudgetDbContext>
{
    /// <inheritdoc />
    public BudgetDbContext CreateDbContext(string[] args)
    {
        // Build minimal configuration looking for appsettings.json in API project or current.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = configuration.GetConnectionString("BudgetDb") ??
                 "Host=localhost;Database=budget_experiment;Username=budget_app;Password=dev_missing;Include Error Detail=true";

        var optionsBuilder = new DbContextOptionsBuilder<BudgetDbContext>();
        optionsBuilder.UseNpgsql(cs);
        return new BudgetDbContext(optionsBuilder.Options);
    }
}
