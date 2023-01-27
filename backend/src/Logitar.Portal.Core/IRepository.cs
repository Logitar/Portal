namespace Logitar.Portal.Core
{
  public interface IRepository
  {
    Task<T?> LoadAsync<T>(AggregateId id, CancellationToken cancellationToken = default) where T : AggregateRoot;
    Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<AggregateId> ids, CancellationToken cancellationToken = default) where T : AggregateRoot;
    Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken = default);
    Task SaveAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken = default);
  }
}
