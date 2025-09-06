// <copyright file="Program.cs" company="Fortinbra">
// Copyright (c) 2025 Fortinbra (becauseimclever.com). All rights reserved.

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

        // Controllers will be registered later when API endpoints are added.
        builder.Services.AddControllers();

        // OpenAPI temporarily skipped; re-enable with builder.Services.AddEndpointsApiExplorer() and Swagger later.
        var app = builder.Build();

        // Swagger UI wiring will be added later (development only).
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        await app.RunAsync().ConfigureAwait(false);
    }
}
