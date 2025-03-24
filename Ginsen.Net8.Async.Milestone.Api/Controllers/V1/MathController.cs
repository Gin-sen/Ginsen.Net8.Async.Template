using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace Ginsen.Net8.Async.Milestone.Api.Controllers.V1
{
  /// <summary>
  /// Controller for handling AzureTable operations.
  /// </summary>
  [ApiVersion(1)]
  [Route("api/v{v:apiVersion}/[controller]/[action]")]
  [ApiController]
  public class MathController : ControllerBase
  {
    private readonly ILogger<MathController> _logger;

    /// <summary>
    /// Controller for handling dummy operations.
    /// </summary>
    public MathController(ILogger<MathController> logger)
    {
      _logger = logger;
    }

    // /api/values2/divide/1/2
    /// <summary>
    /// Returns the result of dividing the numerator by the denominator.
    /// </summary>
    /// <param name="Numerator"></param>
    /// <param name="Denominator"></param>
    /// <returns></returns>
    [HttpGet("{Numerator}/{Denominator}")]
    public IActionResult Divide(double Numerator, double Denominator)
    {
      if (Denominator == 0)
      {
          return BadRequest();
      }

      return Ok(Numerator / Denominator);
    }

    // /api/values2 /squareroot/4
    /// <summary>
    /// Returns the square root of the radicand.
    /// </summary>
    /// <param name="radicand"></param>
    /// <returns></returns>
    [HttpGet("{radicand}")]
    public IActionResult SquareRoot(double radicand)
    {
      if (radicand < 0)
      {
          return BadRequest();
      }

      return Ok(Math.Sqrt(radicand));
    }
}
}
