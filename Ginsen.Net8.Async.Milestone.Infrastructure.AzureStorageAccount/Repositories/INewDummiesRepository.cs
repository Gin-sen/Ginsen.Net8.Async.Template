using Ginsen.Net8.Async.Milestone.Infrastructure.AzureStorageAccount.Entities;

namespace Ginsen.Net8.Async.Milestone.Infrastructure.AzureStorageAccount.Repositories;

public interface INewDummiesRepository
{
    Task<DummyEntity> GetEntityAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default);
    Task<DummyEntity> UpsertEntityAsync(DummyEntity entity, CancellationToken cancellationToken = default);
    Task DeleteEntityAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default);
}