namespace Ginsen.Net8.Async.Milestone.Contracts.Http.V1.Weather
{
  /// <summary>
  /// Represents a weather forecast for a specific date.
  /// </summary>
  public class WeatherForecast
  {
    /// <summary>
    /// The date of the weather forecast.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// The temperature in Celsius.
    /// </summary>
    public int TemperatureC { get; set; }

    /// <summary>
    /// The temperature in Fahrenheit.
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    /// <summary>
    /// Summary of the weather forecast.
    /// </summary>
    public string? Summary { get; set; }
  }
}
