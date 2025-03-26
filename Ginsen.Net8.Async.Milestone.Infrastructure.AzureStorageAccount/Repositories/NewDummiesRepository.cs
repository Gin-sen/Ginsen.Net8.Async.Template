using Azure.Data.Tables;
using Ginsen.Net8.Async.Milestone.Infrastructure.AzureStorageAccount.Entities;
using Microsoft.Extensions.Logging;

namespace Ginsen.Net8.Async.Milestone.Infrastructure.AzureStorageAccount.Repositories;

public class NewDummiesRepository : INewDummiesRepository
{
  private const string TableName = "Dummies";
  private readonly TableServiceClient _tableServiceClient;
  private readonly ILogger<NewDummiesRepository> _logger;

  public NewDummiesRepository(TableServiceClient tableServiceClient, ILogger<NewDummiesRepository> logger)
  {
    _tableServiceClient = tableServiceClient;
    _logger = logger;
  }
  public async Task<DummyEntity> GetEntityAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default){
    TableClient tableClient = _tableServiceClient.GetTableClient(TableName);
    return await tableClient.GetEntityAsync<DummyEntity>(partitionKey, rowKey, cancellationToken: cancellationToken);
  }
  public async Task<DummyEntity> UpsertEntityAsync(DummyEntity entity, CancellationToken cancellationToken = default){
    TableClient tableClient = _tableServiceClient.GetTableClient(TableName);
    await tableClient.UpsertEntityAsync<DummyEntity>(entity, cancellationToken: cancellationToken);
    return entity;
  }
  public async Task DeleteEntityAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default){
    TableClient tableClient = _tableServiceClient.GetTableClient(TableName);
    await tableClient.DeleteEntityAsync(partitionKey, rowKey, cancellationToken: cancellationToken);
  }
}