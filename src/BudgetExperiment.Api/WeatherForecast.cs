// <copyright file="WeatherForecast.cs" company="Fortinbra">
// Copyright (c) 2025 Fortinbra (becauseimclever.com). All rights reserved.

namespace BudgetExperiment.Api;

/// <summary>
/// Represents a sample weather forecast projection.
/// </summary>
internal sealed class WeatherForecast
{
    /// <summary>Gets or sets the forecast date.</summary>
    public DateOnly Date { get; set; }

    /// <summary>Gets or sets the temperature in Celsius.</summary>
    public int TemperatureC { get; set; }

    /// <summary>Gets the temperature in Fahrenheit.</summary>
    public int TemperatureF => 32 + (int)(this.TemperatureC / 0.5556);

    /// <summary>Gets or sets a textual summary.</summary>
    public string? Summary { get; set; }
}
