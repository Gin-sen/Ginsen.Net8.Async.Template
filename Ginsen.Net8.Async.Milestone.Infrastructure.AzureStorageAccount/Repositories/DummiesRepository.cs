using Ginsen.Net8.Async.Milestone.Application.Repositories;
using Ginsen.Net8.Async.Milestone.Infrastructure.AzureStorageAccount.Entities;
using Azure.Data.Tables;
using Ginsen.Net8.Async.Milestone.Domain.Dummies;
using Microsoft.Extensions.Logging;


namespace Ginsen.Net8.Async.Milestone.Infrastructure.AzureStorageAccount.Repositories
{
  public class DummiesRepository : IDummiesRepository
  {
    private readonly TableClient _tableClient;
    private readonly ILogger<DummiesRepository> _logger;

    public DummiesRepository(TableServiceClient serviceClient, ILogger<DummiesRepository> logger)
    {
      _tableClient = serviceClient.GetTableClient("Dummies");
      _logger = logger;
    }

    public async Task AddAsync(Dummy dummy, CancellationToken cancellationToken = default)
    {
      await _tableClient.AddEntityAsync(new DummyEntity(dummy), cancellationToken);
    }

    public async Task<Dummy?> GetAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default)
    {
      try {
        var result = (await _tableClient.GetEntityAsync<DummyEntity>(partitionKey, rowKey, cancellationToken: cancellationToken)).Value;
        if (result == null)
          return null;
        return new Dummy(result.PartitionKey, result.RowKey, result.Message);
      } catch (Azure.RequestFailedException ex) when (ex.ErrorCode == "ResourceNotFound" && ex.Status == 404)
      {
        _logger.LogInformation("Entity not found ({PartitionKey}/{RowKey})", partitionKey, rowKey);
      }
      return null;
    }
    public async Task<Dummy?> GetAsync(Dummy dummy, CancellationToken cancellationToken = default)
    {
      try {
        var result = (await _tableClient.GetEntityAsync<DummyEntity>(dummy.PartitionKey.ToString(), dummy.RowKey.ToString(), cancellationToken: cancellationToken)).Value;
        if (result == null)
          return null;
        dummy.Message = result.Message;
        return dummy;
      } catch (Azure.RequestFailedException ex) when (ex.ErrorCode == "ResourceNotFound" && ex.Status == 404)
      {
        _logger.LogInformation("Entity not found ({PartitionKey}/{RowKey})", dummy.PartitionKey, dummy.RowKey);
      }
      return null;
    }


    public async Task<IEnumerable<Dummy>> GetAllAsync(string partitionKey, CancellationToken cancellationToken = default)
    {
      var query = _tableClient.QueryAsync<DummyEntity>(e => e.PartitionKey == partitionKey, cancellationToken: cancellationToken);
      var results = new List<Dummy>();
      await foreach (var entity in query)
      {
        results.Add(new Dummy(entity.PartitionKey, entity.RowKey, entity.Message));
      }
      return results;
    }

    public async Task AddOrUpdateAsync(Dummy dummy, CancellationToken cancellationToken = default)
    {
      try {
        var result = (await _tableClient.GetEntityAsync<DummyEntity>(dummy.PartitionKey.ToString(), dummy.RowKey.ToString(), cancellationToken: cancellationToken)).Value;
        result.Message = dummy.Message;
        await _tableClient.UpdateEntityAsync(result, result.ETag, TableUpdateMode.Replace, cancellationToken);
      } catch (Azure.RequestFailedException ex) when (ex.ErrorCode == "ResourceNotFound" && ex.Status == 404)
      {
        _logger.LogInformation("Entity not found ({PartitionKey}/{RowKey}) while updating, creating it", dummy.PartitionKey, dummy.RowKey);
        await AddAsync(dummy, cancellationToken);
      }
    }

    public async Task DeleteAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default)
    {
      await _tableClient.DeleteEntityAsync(partitionKey, rowKey, cancellationToken: cancellationToken);
    }
  }
}