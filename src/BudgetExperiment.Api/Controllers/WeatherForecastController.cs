// Copyright (c) 2025 Fortinbra (becauseimclever.com). All rights reserved.

namespace BudgetExperiment.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Provides sample weather forecast data (placeholder).
/// </summary>
[ApiController]
[Route("[controller]")]
internal sealed class WeatherForecastController : ControllerBase
{
    private static readonly string[] _summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
    ];

    /// <summary>
    /// Gets sample weather forecast values.
    /// </summary>
    /// <returns>Collection of <see cref="WeatherForecast"/> records.</returns>
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = _summaries[Random.Shared.Next(_summaries.Length)],
        })
        .ToArray();
    }
}
