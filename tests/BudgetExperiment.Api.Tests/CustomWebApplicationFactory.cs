namespace BudgetExperiment.Api.Tests;

using BudgetExperiment.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Custom factory that configures an in-memory database for tests.
/// </summary>
public sealed class CustomWebApplicationFactory : WebApplicationFactory<object>
{
    /// <summary>Creates an <see cref="HttpClient"/> for the API.</summary>
    /// <returns>Client.</returns>
    public HttpClient CreateApiClient() => this.CreateClient();

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registration (Postgres)
            var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<BudgetDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            // Replace with EF Core InMemory for integration tests
            services.AddDbContext<BudgetDbContext>(options => options.UseInMemoryDatabase("TestDb"));

            // Build provider to create database
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BudgetDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
