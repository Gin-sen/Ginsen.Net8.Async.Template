namespace Ginsen.Net8.Async.Milestone.Api.Contracts.V1.Dummies
{
    /// <summary>
    /// Represents a dummy response with a message.
    /// </summary>
    public record GetDummy(Guid PartitionKey, Guid RowKey, string? Message);
}