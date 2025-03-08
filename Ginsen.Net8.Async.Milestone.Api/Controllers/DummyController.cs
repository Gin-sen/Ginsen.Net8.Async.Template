using Microsoft.AspNetCore.Mvc;
using Ginsen.Net8.Async.Milestone.Contracts.Http.V1.Dummies;
using Asp.Versioning;

namespace Ginsen.Net8.Async.Milestone.Api.Controllers
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
    public async Task<IActionResult> GetOkAsync(
      CancellationToken cancellationToken)
    {
      if (_logger.IsEnabled(LogLevel.Information))
      {
        _logger.LogInformation("Ok");
      }
      await Task.Delay(1_000, cancellationToken);
      return Ok(new GetDummy());
    }
  }
}
