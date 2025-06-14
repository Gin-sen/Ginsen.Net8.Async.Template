using Azure.Data.Tables;
using Ginsen.Net8.Async.Milestone.Infrastructure.AzureStorageAccount.Entities;
using Microsoft.Extensions.Logging;

namespace Ginsen.Net8.Async.Milestone.Infrastructure.AzureStorageAccount.Repositories;

public class DummiesRepository : IDummiesRepository
{
  private const string TableName = "Dummies";
  private readonly TableServiceClient _tableServiceClient;
  private readonly ILogger<DummiesRepository> _logger;

  public DummiesRepository(TableServiceClient tableServiceClient, ILogger<DummiesRepository> logger)
  {
    _tableServiceClient = tableServiceClient;
    _logger = logger;
    if (_logger.IsEnabled(LogLevel.Information))
    {
      _logger.LogInformation("Creating class {type}", typeof(DummiesRepository).Name);
    }
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

  public async Task<IEnumerable<DummyEntity>> GetEntityListAsync(
    string partitionKey = "*",
    string rowKey = "*",
    int pageNumber = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default)
  {
    TableClient tableClient = _tableServiceClient.GetTableClient(TableName);
    var query = tableClient.QueryAsync<DummyEntity>(e => e.PartitionKey == partitionKey && e.RowKey == rowKey, maxPerPage: pageSize, cancellationToken: cancellationToken);
    var result = new List<DummyEntity>();
    await foreach (var entity in query.WithCancellation(cancellationToken))
    {
        result.Add(entity);
    }
    return result;

  }
}