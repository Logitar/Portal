using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application
{
  public interface IRepository
  {
    Task<T?> LoadAsync<T>(string id, CancellationToken cancellationToken = default) where T : AggregateRoot;
    Task<T?> LoadAsync<T>(AggregateId id, CancellationToken cancellationToken = default) where T : AggregateRoot;
    Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<string> ids, CancellationToken cancellationToken = default) where T : AggregateRoot;
    Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<AggregateId> ids, CancellationToken cancellationToken = default) where T : AggregateRoot;
    Task<Configuration?> LoadConfigurationAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken = default);
    Task SaveAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken = default);
  }
}
