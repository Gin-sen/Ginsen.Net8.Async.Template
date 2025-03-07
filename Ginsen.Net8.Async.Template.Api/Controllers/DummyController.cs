using Microsoft.AspNetCore.Mvc;
using Ginsen.Net8.Async.Template;
using System.Net;

namespace Ginsen.Net8.Async.Template.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DummyController : ControllerBase
  {
    private readonly ILogger<DummyController> _logger;
    public DummyController(ILogger<DummyController> logger)
    {
      _logger = logger;
    }

    [HttpGet("GetOk")]
    public async Task<IActionResult> GetOkAsync(
      CancellationToken cancellationToken)
    {
      if (_logger.IsEnabled(LogLevel.Information))
      {
        _logger.LogInformation("Ok");
      }
      await Task.Delay(1_000, cancellationToken);
      return Ok(new {});
    }
  }
}
