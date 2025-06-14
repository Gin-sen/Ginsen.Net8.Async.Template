using Ginsen.Net8.Async.Milestone.Api.Contracts.V1.Dummies;

public interface IAzureTablesHttpRepository
{
    Task<GetDummiesResult> GetAllAsync(string? partitionKey = "*", string? rowKey = "*");
    Task<GetDummyResult> GetByIdAsync(string id);
    Task UpsertAsync(CreateDummyRequest entity);
}