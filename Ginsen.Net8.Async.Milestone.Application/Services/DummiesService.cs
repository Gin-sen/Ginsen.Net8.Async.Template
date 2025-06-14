
using Ginsen.Net8.Async.Milestone.Domain.Dummies;
using Ginsen.Net8.Async.Milestone.Infrastructure.AzureStorageAccount.Entities;
using Ginsen.Net8.Async.Milestone.Infrastructure.AzureStorageAccount.Repositories;
using Microsoft.Extensions.Logging;

namespace Ginsen.Net8.Async.Milestone.Application.Repositories;

public class DummiesService : IDummiesService
{
  private readonly IDummiesRepository _dummiesRepository;
  private readonly ILogger<DummiesService> _logger;

  public DummiesService(IDummiesRepository dummiesRepository, ILogger<DummiesService> logger)
  {
    _dummiesRepository = dummiesRepository;
    _logger = logger;
  }

  public async Task<Dummy> UpsertAsync(string partitionKey, string rowKey, string? message, CancellationToken cancellationToken = default)
  {
    var result = await _dummiesRepository.UpsertEntityAsync(new DummyEntity(partitionKey, rowKey, message), cancellationToken);
    return new Dummy(result.PartitionKey, result.RowKey, result.Message);
  }

  public async Task<Dummy> GetAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default)
  {
    var result = await _dummiesRepository.GetEntityAsync(partitionKey, rowKey, cancellationToken);
    return new Dummy(result.PartitionKey, result.RowKey, result.Message);
  }

  public async Task<IEnumerable<Dummy>> GetListAsync(string? partitionKey = "*", string? rowKey = "*", CancellationToken cancellationToken = default)
  {
    var result = await _dummiesRepository.GetEntityListAsync(partitionKey ?? "*", rowKey ?? "*", 1, 10, cancellationToken);
    return result.Select(e => new Dummy(e.PartitionKey, e.RowKey, e.Message));
  }
}