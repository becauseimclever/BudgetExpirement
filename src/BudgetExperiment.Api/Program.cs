// <copyright file="Program.cs" company="Fortinbra">
// Copyright (c) 2025 Fortinbra (becauseimclever.com). All rights reserved.

using BudgetExperiment.Application;
using BudgetExperiment.Infrastructure;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Scalar.AspNetCore;

/// <summary>
/// Application entry point.
/// </summary>
public partial class Program
{
    /// <summary>
    /// Main entry point. Configures and runs the ASP.NET Core host.
    /// </summary>
    /// <param name="args">Runtime arguments.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add controllers + OpenAPI (ASP.NET Core built-in OpenAPI services).
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        // Application & Infrastructure
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddHealthChecks();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("dev", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        });

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseCors("dev");

        // Serve Blazor WebAssembly client (static web assets from referenced Client project).
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        // Expose OpenAPI document (AspNetCore.OpenApi)
        app.MapOpenApi();
        app.MapScalarApiReference();

        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health");

        // Fallback to index.html for client-side routes.
        app.MapFallbackToFile("index.html");

        // Custom exception handling
        app.UseMiddleware<BudgetExperiment.Api.Middleware.ExceptionHandlingMiddleware>();

        // Apply pending migrations and seed data if needed.
        using (var scope = app.Services.CreateScope())
        {
            try
            {
                var db = scope.ServiceProvider.GetRequiredService<BudgetDbContext>();

                // Apply any pending migrations
                await db.Database.MigrateAsync().ConfigureAwait(false);

                // Seed bi-weekly pay schedule if none exist.
                if (!db.PaySchedules.Any())
                {
                    var seed = BudgetExperiment.Domain.PaySchedule.CreateBiWeekly(new DateOnly(2025, 8, 29), BudgetExperiment.Domain.MoneyValue.Create("USD", 1500m));
                    db.PaySchedules.Add(seed);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }

                // Seed monthly bill schedule if none exist.
                if (!db.BillSchedules.Any())
                {
                    var billSeed = BudgetExperiment.Domain.BillSchedule.CreateMonthly("Rent", BudgetExperiment.Domain.MoneyValue.Create("USD", 2400m), new DateOnly(2025, 9, 4));
                    db.BillSchedules.Add(billSeed);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }

                // Seed expense data if none exist.
                if (!db.Expenses.Any())
                {
                    var expenseSeed = BudgetExperiment.Domain.Expense.Create(
                        "Groceries at HEB",
                        BudgetExperiment.Domain.MoneyValue.Create("USD", 150m),
                        new DateOnly(2025, 9, 8),
                        "Groceries");
                    db.Expenses.Add(expenseSeed);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                // Log minimal info (logging pipeline configured later) and rethrow.
                Console.Error.WriteLine($"Database ensure/create failed: {ex.Message}");
                throw;
            }
        }

        await app.RunAsync().ConfigureAwait(false);
    }
}
