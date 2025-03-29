using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Ginsen.Net8.Async.Milestone.Api.Contracts.V1.Weather;

namespace Ginsen.Net8.Async.Milestone.Api.Endpoints.V1
{
  /// <summary>
  /// Endpoints for handling Weather operations.
  /// </summary>
  public static class WeatherEndpoints
  {
    /// <summary>
    /// Maps the Weather group endpoints.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="apiVersionSet"></param>
    public static void MapWeatherEndpoints(WebApplication app, ApiVersionSet apiVersionSet)
    {
      app.MapGroup("api/v{v:apiVersion}/weather")
        .MapWeatherEndpoints()
        .WithTags("Weather")
        .WithOpenApi()
        .WithApiVersionSet(apiVersionSet);

    }
    /// <summary>
    /// Maps the Weather endpoints.
    /// </summary>
    public static RouteGroupBuilder MapWeatherEndpoints(this RouteGroupBuilder group)
    {
      group.MapGet("/",
        async (
          [FromServices] ILogger<Program> logger,
          CancellationToken cancellationToken,
          [FromQuery]int days = 5) => {
            if (logger.IsEnabled(LogLevel.Information))
            {
              logger.LogInformation("Getting {WheatherCount} wheather scores", days);
            }
            if (days < 1)
            {
              if (logger.IsEnabled(LogLevel.Warning))
              {
                logger.LogWarning("Invalid number of days: {Days}", days);
              }
              return Results.BadRequest(Array.Empty<WeatherForecast>());
            }
            var result = Enumerable.Range(1, days).Select(static index => new WeatherForecast
            {
              Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
              TemperatureC = Random.Shared.Next(-20, 55),
              Summary = Enum.GetNames<Summaries>()[Random.Shared.Next(Enum.GetValues(typeof(Summaries)).Length)] //Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            if (logger.IsEnabled(LogLevel.Debug))
            {
              logger.LogDebug("Wheathers : {@WheatherList}", result);
            }
            await Task.Delay(1_000);
            return Results.Ok(result);
          })
        .WithName("GetWeatherForecast")
        .Produces<IEnumerable<WeatherForecast>>(StatusCodes.Status200OK)
        .Produces<IEnumerable<WeatherForecast>>(StatusCodes.Status400BadRequest)
        .MapToApiVersion(1);

      return group;
    }
  }
}