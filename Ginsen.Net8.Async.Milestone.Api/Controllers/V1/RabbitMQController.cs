using Asp.Versioning;
using Ginsen.Net8.Async.Milestone.Contracts.Messaging.V1;
using Microsoft.AspNetCore.Mvc;

namespace Ginsen.Net8.Async.Milestone.Api.Controllers.V1
{
    /// <summary>
    /// Controller for handling RabbitMQ related operations.
    /// </summary>
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    public class RabbitMQController : ControllerBase
    {
        private readonly ILogger<RabbitMQController> _logger;

        /// <summary>
        /// Controller for handling RabbitMQ related operations.
        /// </summary>
        public RabbitMQController(ILogger<RabbitMQController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Publish a RabbitMQ message after some treatment.
        /// </summary>
        [HttpPost("Publish")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FooMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PublishAsync(
            [FromBody] FooMessage message,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(message.Content))
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning("Message is empty");
                }
                return BadRequest("Message is empty");
            }
            // do something
            await Task.Delay(1_000, cancellationToken);
            // then send a message to RabbitMQ
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Message sent to RabbitMQ");
            }
            return Ok(message);
        }
    }
}
