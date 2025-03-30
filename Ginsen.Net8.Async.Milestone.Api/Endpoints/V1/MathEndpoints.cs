using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Asp.Versioning;
using Asp.Versioning.Builder;

namespace Ginsen.Net8.Async.Milestone.Api.Endpoints.V1
{
  /// <summary>
  /// Endpoints for handling Math operations.
  /// </summary>
  public static class MathEndpoints
  {
    /// <summary>
    /// Maps the Math group endpoints.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="apiVersionSet"></param>
    public static void MapMathEndpoints(WebApplication app, ApiVersionSet apiVersionSet)
    {
      app.MapGroup("api/v{v:apiVersion}/math")
        .MapMathEndpoints()
        .WithTags("Math")
        .WithOpenApi()
        .WithApiVersionSet(apiVersionSet);

    }
    /// <summary>
    /// Maps the Math endpoints.
    /// </summary>
    public static RouteGroupBuilder MapMathEndpoints(this RouteGroupBuilder group)
    {
      group.MapGet("/{Numerator}/{Denominator}",
        (
          [FromServices] ILogger<Program> logger,
          [FromRoute] double Numerator,
          [FromRoute] double Denominator,
          CancellationToken cancellationToken) => {
            // Yes, it's required to use a dictionary. See https://nblumhardt.com/2016/11/ilogger-beginscope/
            using (logger.BeginScope(new Dictionary<string, object>
            {
                ["Denominator"] = Denominator,
                ["OperationType"] = "Divide",
            }))
            // Denominator and OperationType are set for all logging events in these brackets
            
            if (Denominator == 0)
            {
              if (logger.IsEnabled(LogLevel.Error))
              {
                  logger.LogError("Denominator is zero");
              }
              return Results.BadRequest();
            }
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Dividing {Numerator} by {Denominator}", Numerator, Denominator);
            }
            return Results.Ok(Numerator / Denominator);
          })
        .WithName("Divide")
        .Produces<double>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .MapToApiVersion(1);

      group.MapGet("/{radicand}",
        (
          [FromRoute] double radicand,
          CancellationToken cancellationToken) => {
            if (radicand < 0)
            {
                return Results.BadRequest();
            }
            return Results.Ok(Math.Sqrt(radicand));
          })
        .WithName("SquareRoot")
        .Produces<double>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .MapToApiVersion(1);

      return group;
    }
  }
}