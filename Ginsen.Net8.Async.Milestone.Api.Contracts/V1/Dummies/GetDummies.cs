
using Ginsen.Net8.Async.Milestone.Api.Contracts.V1.Dummies;


/// <summary>
/// GetDummies Request DTO
/// </summary>
/// <param name="Message"></param>
public record GetDummiesRequest(string? Message);

/// <summary>
/// GetDummies Result DTO
/// </summary>
/// <param name="Dummies"></param>
public record GetDummiesResult(IEnumerable<GetDummyResult> Dummies);