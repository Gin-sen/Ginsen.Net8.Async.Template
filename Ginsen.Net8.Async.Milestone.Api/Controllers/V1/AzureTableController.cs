using Microsoft.AspNetCore.Mvc;
using Ginsen.Net8.Async.Milestone.Api.Contracts.V1.Dummies;
using Asp.Versioning;
using Ginsen.Net8.Async.Milestone.Application.Repositories;

namespace Ginsen.Net8.Async.Milestone.Api.Controllers.V1
{
  /// <summary>
  /// Controller for handling AzureTable operations.
  /// </summary>
  [ApiVersion(1)]
  [Route("api/v{v:apiVersion}/[controller]")]
  [ApiController]
  public class AzureTableController : ControllerBase
  {
    private readonly ILogger<AzureTableController> _logger;
    private readonly IDummiesService _dummiesService;

    /// <summary>
    /// Controller for handling dummy operations.
    /// </summary>
    public AzureTableController(ILogger<AzureTableController> logger, IDummiesService dummiesService)
    {
      _logger = logger;
      _dummiesService = dummiesService;
    }

    /// <summary>
    /// Get a dummy object.
    /// </summary>
    /// <param name="partitionKey"></param>
    /// <param name="rowKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{partitionKey}/{rowKey}")]
    [Produces( "application/json" )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDummy))]
    public async Task<IActionResult> GetAsync([FromRoute]Guid partitionKey, [FromRoute]Guid rowKey,
      CancellationToken cancellationToken)
    {
      if (_logger.IsEnabled(LogLevel.Information))
      {
        _logger.LogInformation("Getting entity {PartitionKey}/{RowKey}", partitionKey, rowKey);
      }
      var result = await _dummiesService.GetAsync(partitionKey.ToString(), rowKey.ToString(), cancellationToken);
      if (result == null)
        return NotFound();
      return Ok(new GetDummy(result.PartitionKey, result.RowKey, result.Message));
    }
  }
}
