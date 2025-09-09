namespace BudgetExperiment.Api.Tests;

using BudgetExperiment.Api;
using BudgetExperiment.Infrastructure;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Custom factory for API integration tests using the configured database infrastructure.
/// </summary>
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    /// <summary>Creates an <see cref="HttpClient"/> for the API.</summary>
    /// <returns>Client.</returns>
    public HttpClient CreateApiClient() => this.CreateClient();

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Use real configured infrastructure (PostgreSQL). Ensure database exists/migrated externally.
    }
}
