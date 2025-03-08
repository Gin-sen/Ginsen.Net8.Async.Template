namespace Ginsen.Net8.Async.Milestone.Contracts.Messaging.V1
{
    /// <summary>
    /// Represents a message to be sent to a RabbitMQ exchange.
    /// </summary>
    public class FooMessage
    {
        /// <summary>
        /// Gets or sets the message content.
        /// </summary>
        public string? Content { get; set; }
    }
}