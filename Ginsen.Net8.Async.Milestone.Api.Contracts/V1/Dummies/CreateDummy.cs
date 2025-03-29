namespace Ginsen.Net8.Async.Milestone.Api.Contracts.V1.Dummies;
/// <summary>
/// CreateDummy Result DTO
/// </summary>
/// <param name="PartitionKey"></param>
/// <param name="RowKey"></param>
/// <param name="Message"></param>
public record CreateDummyResult(Guid PartitionKey, Guid RowKey, string? Message);

/// <summary>
/// CreateDummy Request DTO
/// </summary>
/// <param name="Message"></param>
public record CreateDummyRequest(string? Message);