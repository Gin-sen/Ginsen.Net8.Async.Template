using Asp.Versioning;
using Ginsen.Net8.Async.Milestone.Contracts.Http.V1.Weather;
using Microsoft.AspNetCore.Mvc;

namespace Ginsen.Net8.Async.Milestone.Api.Controllers.V1
{
  /// <summary>
  /// Controller to handle weather forecast requests.
  /// </summary>
  [ApiVersion(1)]
  [ApiController]
  [Route("api/v{v:apiVersion}/[controller]")]
  public class WeatherForecastController : ControllerBase
  {
    // private static readonly string[] Summaries = Enum.GetValues<Summaries>();
    private readonly ILogger<WeatherForecastController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherForecastController"/> class.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
      _logger = logger;
    }

    /// <summary>
    /// Gets the weather forecast for the specified number of days.
    /// </summary>
    /// <param name="days">The number of days to get the forecast for.</param>
    /// <returns>A list of weather forecasts.</returns>
    [HttpGet(Name = "GetWeatherForecast")]
    [Produces( "application/json" )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WeatherForecast>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<WeatherForecast>))]
    public async Task<IActionResult> Get([FromQuery]int days = 5)
    {
      if (_logger.IsEnabled(LogLevel.Information))
      {
        _logger.LogInformation("Getting {WheatherCount} wheather scores", days);
      }
      if (days < 1)
      {
        if (_logger.IsEnabled(LogLevel.Warning))
        {
          _logger.LogWarning("Invalid number of days: {Days}", days);
        }
        return BadRequest(Array.Empty<WeatherForecast>());
      }
      var result = Enumerable.Range(1, days).Select(static index => new WeatherForecast
      {
        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = Enum.GetNames<Summaries>()[Random.Shared.Next(Enum.GetValues(typeof(Summaries)).Length)] //Summaries[Random.Shared.Next(Summaries.Length)]
      })
      .ToArray();

      if (_logger.IsEnabled(LogLevel.Debug))
      {
        _logger.LogDebug("Wheathers : {@WheatherList}", result);
      }
      await Task.Delay(1_000);
      return Ok(result);
    }
  }
}
