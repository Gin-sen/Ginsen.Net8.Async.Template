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
          [FromRoute] double Numerator,
          [FromRoute] double Denominator,
          CancellationToken cancellationToken) => {
            if (Denominator == 0)
            {
                return Results.BadRequest();
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