// <copyright file="Program.cs" company="Fortinbra">
// Copyright (c) 2025 Fortinbra (becauseimclever.com). All rights reserved.

using BudgetExperiment.Application;
using BudgetExperiment.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

using Scalar.AspNetCore;

/// <summary>
/// Application entry point.
/// </summary>
public static class Program
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

        // Expose OpenAPI document (AspNetCore.OpenApi)
        app.MapOpenApi();
        app.MapScalarApiReference();

        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health");

        // Temporary minimal endpoints for smoke testing pay schedules.
        app.MapPost("/api/v1/payschedules/weekly", async (DateOnly anchor, BudgetExperiment.Application.PaySchedules.IPayScheduleService svc, CancellationToken ct) =>
            {
                var id = await svc.CreateWeeklyAsync(anchor, ct).ConfigureAwait(false);
                return Results.Created($"/api/v1/payschedules/{id}", new
                {
                    id,
                });
            });

        app.MapGet("/api/v1/payschedules/{id:guid}/occurrences", async (Guid id, DateOnly start, DateOnly end, BudgetExperiment.Application.PaySchedules.IPayScheduleService svc, CancellationToken ct) =>
            {
                var occ = await svc.GetOccurrencesAsync(id, start, end, ct).ConfigureAwait(false);
                return Results.Ok(occ);
            });

        // Bill schedule endpoints
        app.MapPost("/api/v1/billschedules/monthly", async (string name, string currency, decimal amount, DateOnly anchor, BudgetExperiment.Application.BillSchedules.IBillScheduleService svc, CancellationToken ct) =>
            {
                var id = await svc.CreateMonthlyAsync(name, BudgetExperiment.Domain.MoneyValue.Create(currency, amount), anchor, ct).ConfigureAwait(false);
                return Results.Created($"/api/v1/billschedules/{id}", new
                {
                    id,
                });
            });

        app.MapGet("/api/v1/billschedules/{id:guid}/occurrences", async (Guid id, DateOnly start, DateOnly end, BudgetExperiment.Application.BillSchedules.IBillScheduleService svc, CancellationToken ct) =>
            {
                var occ = await svc.GetOccurrencesAsync(id, start, end, ct).ConfigureAwait(false);
                return Results.Ok(occ);
            });
        await app.RunAsync().ConfigureAwait(false);
    }
}
