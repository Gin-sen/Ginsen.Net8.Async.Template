using Microsoft.AspNetCore.Mvc;
using Ginsen.Net8.Async.Milestone.Api.Contracts.V1.Dummies;
using Asp.Versioning;

namespace Ginsen.Net8.Async.Milestone.Api.Controllers.V1
{
  /// <summary>
  /// Controller for handling dummy operations.
  /// </summary>
  [ApiVersion(1)]
  [Route("api/v{v:apiVersion}/[controller]")]
  [ApiController]
  public class DummyController : ControllerBase
  {
    private readonly ILogger<DummyController> _logger;

    /// <summary>
    /// Controller for handling dummy operations.
    /// </summary>
    public DummyController(ILogger<DummyController> logger)
    {
      _logger = logger;
    }

    /// <summary>
    /// Get a dummy object.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetOk")]
    [Produces( "application/json" )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDummy))]
    public IActionResult GetOkAsync(
      CancellationToken cancellationToken)
    {
      if (_logger.IsEnabled(LogLevel.Information))
      {
        _logger.LogInformation("Ok");
      }
      return Ok(new GetDummy(Guid.NewGuid(), Guid.NewGuid(), "Dummy"));
    }
  }
}
