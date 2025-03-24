using Ginsen.Net8.Async.Milestone.Domain.Dummies;

namespace Ginsen.Net8.Async.Milestone.Application.Repositories;
public interface IDummiesRepository
{
  Task AddAsync(Dummy entity, CancellationToken cancellationToken);
  Task<Dummy?> GetAsync(string partitionKey, string rowKey, CancellationToken cancellationToken);
  Task<Dummy?> GetAsync(Dummy dummy, CancellationToken cancellationToken);
  Task<IEnumerable<Dummy>> GetAllAsync(string partitionKey, CancellationToken cancellationToken);
  Task AddOrUpdateAsync(Dummy dummy, CancellationToken cancellationToken);
  Task DeleteAsync(string partitionKey, string rowKey, CancellationToken cancellationToken);
}
