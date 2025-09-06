// <copyright file="Program.cs" company="Fortinbra">
// Copyright (c) 2025 Fortinbra (becauseimclever.com). All rights reserved.

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

        // Application services
        builder.Services.AddSingleton<BudgetExperiment.Application.PaySchedules.IPayScheduleService, BudgetExperiment.Application.PaySchedules.InMemoryPayScheduleService>();
        builder.Services.AddSingleton<BudgetExperiment.Application.BillSchedules.IBillScheduleService, BudgetExperiment.Application.BillSchedules.InMemoryBillScheduleService>();

        var app = builder.Build();

        app.UseHttpsRedirection();

        // Expose OpenAPI document (AspNetCore.OpenApi)
        app.MapOpenApi();
        app.MapScalarApiReference();

        app.UseAuthorization();
        app.MapControllers();

        // Temporary minimal endpoints for smoke testing pay schedules.
        app.MapPost("/api/v1/payschedules/weekly", (DateOnly anchor, BudgetExperiment.Application.PaySchedules.IPayScheduleService svc) =>
            {
                var id = svc.CreateWeekly(anchor);
                return Results.Created($"/api/v1/payschedules/{id}", new
                {
                    id,
                });
            });

        app.MapGet("/api/v1/payschedules/{id:guid}/occurrences", (Guid id, DateOnly start, DateOnly end, BudgetExperiment.Application.PaySchedules.IPayScheduleService svc) =>
            {
                var occ = svc.GetOccurrences(id, start, end);
                return Results.Ok(occ);
            });

        // Bill schedule endpoints
        app.MapPost("/api/v1/billschedules/monthly", (string name, string currency, decimal amount, DateOnly anchor, BudgetExperiment.Application.BillSchedules.IBillScheduleService svc) =>
            {
                var id = svc.CreateMonthly(name, BudgetExperiment.Domain.MoneyValue.Create(currency, amount), anchor);
                return Results.Created($"/api/v1/billschedules/{id}", new
                {
                    id,
                });
            });

        app.MapGet("/api/v1/billschedules/{id:guid}/occurrences", (Guid id, DateOnly start, DateOnly end, BudgetExperiment.Application.BillSchedules.IBillScheduleService svc) =>
            {
                var occ = svc.GetOccurrences(id, start, end);
                return Results.Ok(occ);
            });
        await app.RunAsync().ConfigureAwait(false);
    }
}
