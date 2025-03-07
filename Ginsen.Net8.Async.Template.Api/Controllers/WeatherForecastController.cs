using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace Ginsen.Net8.Async.Template.Api.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class WeatherForecastController : ControllerBase
  {
    private static readonly string[] Summaries = new[]
    {
      "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
      _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get([FromQuery]int days = 5)
    {
      if (_logger.IsEnabled(LogLevel.Information))
      {
        _logger.LogInformation("Getting {WheatherCount} wheather scores", days);
      }
      var result = Enumerable.Range(1, days).Select(index => new WeatherForecast
      {
        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
      })
      .ToArray();

      if (_logger.IsEnabled(LogLevel.Debug))
      {
        _logger.LogDebug("Wheathers : {@WheatherList}", result);
      }
      return result;
    }
  }
}
