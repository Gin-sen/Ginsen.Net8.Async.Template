using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ginsen.Net8.Async.Milestone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitMQController : ControllerBase
    {
        private readonly ILogger<RabbitMQController> _logger;
        public RabbitMQController(ILogger<RabbitMQController> logger)
        {
            _logger = logger;
        }

        [HttpPost("Publish")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PublishAsync(
            [FromBody] string message,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning("Message is empty");
                }
                return BadRequest();
            }
            // do something
            await Task.Delay(1_000, cancellationToken);
            // then send a message to RabbitMQ
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Message sent to RabbitMQ");
            }


            return Ok();
        }
    }
}
