using Ginsen.Net8.Async.Milestone.Domain.Dummies;

namespace Ginsen.Net8.Async.Milestone.Application.Repositories;

public interface IDummiesService
{
  Task<Dummy> GetAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default);
  Task<IEnumerable<Dummy>> GetListAsync(string? partitionKey = "*", string? rowKey = "*", CancellationToken cancellationToken = default);
  Task<Dummy> UpsertAsync(string partitionKey, string rowKey, string? message, CancellationToken cancellationToken = default);
}