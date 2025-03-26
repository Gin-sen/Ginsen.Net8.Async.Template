
using Ginsen.Net8.Async.Milestone.Domain.Dummies;
using Ginsen.Net8.Async.Milestone.Infrastructure.AzureStorageAccount.Repositories;
using Microsoft.Extensions.Logging;

namespace Ginsen.Net8.Async.Milestone.Application.Repositories;

public class DummiesService : IDummiesService
{
  private readonly INewDummiesRepository _dummiesRepository;
  private readonly ILogger<DummiesService> _logger;

  public DummiesService(INewDummiesRepository dummiesRepository, ILogger<DummiesService> logger)
  {
    _dummiesRepository = dummiesRepository;
    _logger = logger;
  }

  public async Task<Dummy> GetAsync(string partitionKey, string rowKey, CancellationToken cancellationToken)
  {
    var result = await _dummiesRepository.GetEntityAsync(partitionKey, rowKey, cancellationToken);
    return new Dummy(result.PartitionKey, result.RowKey, result.Message);
  }
}