using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Ginsen.Net8.Async.Milestone.Api.Contracts.V1.Dummies;
using Asp.Versioning;
using Ginsen.Net8.Async.Milestone.Application.Repositories;
using Asp.Versioning.Builder;

namespace Ginsen.Net8.Async.Milestone.Api.Endpoints.V1
{
  /// <summary>
  /// Endpoints for handling Azure Table operations.
  /// </summary>
  public static class AzureTableEndpoints
  {
    /// <summary>
    /// Maps the Azure Table group endpoints.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="apiVersionSet"></param>
    public static void MapAzureTableEndpoints(WebApplication app, ApiVersionSet apiVersionSet)
    {
      app.MapGroup("api/v{v:apiVersion}/azuretable")
        .MapAzureTableEndpoints()
        .WithTags("Azure Table")
        .WithOpenApi()
        .WithApiVersionSet(apiVersionSet);

    }
    /// <summary>
    /// Maps the Azure Table endpoints.
    /// </summary>
    public static RouteGroupBuilder MapAzureTableEndpoints(this RouteGroupBuilder group)
    {
      group.MapGet("/{partitionKey}/{rowKey}",
        async (
          [FromServices] ILogger<Program> logger,
          [FromServices] IDummiesService dummiesService,
          [FromRoute]Guid partitionKey,
          [FromRoute]Guid rowKey,
          CancellationToken cancellationToken) => {
            if (logger.IsEnabled(LogLevel.Information))
            {
              logger.LogInformation("Getting entity {PartitionKey}/{RowKey}", partitionKey, rowKey);
            }
            var result = await dummiesService.GetAsync(partitionKey.ToString(), rowKey.ToString(), cancellationToken);
            if (result == null)
              return Results.NotFound();
            return Results.Ok(new GetDummyResult(result.PartitionKey, result.RowKey, result.Message));
          })
        .WithName("GetDummy")
        .Produces<GetDummyResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .MapToApiVersion(1);

      group.MapPost("/{partitionKey}/{rowKey}",
        async (
          HttpRequest request,
          [FromServices] ILogger<Program> logger,
          [FromServices] IDummiesService dummiesService,
          [FromRoute]Guid partitionKey,
          [FromRoute]Guid rowKey,
          [FromBody] CreateDummyRequest? dto,
          CancellationToken cancellationToken) => {
            if (logger.IsEnabled(LogLevel.Information))
            {
              logger.LogInformation("Trying to create entity {PartitionKey}/{RowKey} with message {Message}", partitionKey, rowKey, dto?.Message);
            }
            var resultTask = dummiesService.CreateAsync(partitionKey.ToString(), rowKey.ToString(), dto?.Message ?? "", cancellationToken);
            var version = request.HttpContext.GetRequestedApiVersion();
            var result = await resultTask;
            return Results.Created($"/api/v{version}/azuretable/{result.PartitionKey}/{result.RowKey}", new CreateDummyResult(result.PartitionKey, result.RowKey, result.Message));
          })
        .WithName("CreateDummy")
        .Produces<CreateDummyResult>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .MapToApiVersion(1);

      return group;
    }
  }
}
