using Logitar.Portal.Domain;

namespace Logitar.Portal.Application
{
  public interface IRepository<T> where T : Aggregate
  {
    Task SaveAsync(T aggregate, CancellationToken cancellationToken = default);
    Task SaveAsync(IEnumerable<T> aggregates, CancellationToken cancellationToken = default);
  }
}
