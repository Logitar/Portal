namespace Logitar.Portal.Core
{
  public interface IRepository<T> where T : Aggregate
  {
    Task SaveAsync(T aggregate, CancellationToken cancellationToken = default);
    Task SaveAsync(IEnumerable<T> aggregates, CancellationToken cancellationToken = default);
  }
}
