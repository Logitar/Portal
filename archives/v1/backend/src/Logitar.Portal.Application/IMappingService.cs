using Logitar.Portal.Core;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application
{
  public interface IMappingService
  {
    Task<T> MapAsync<T>(Aggregate aggregate, CancellationToken cancellationToken = default);
    Task<T> MapAsync<T>(Session session, CancellationToken cancellationToken = default);
    Task<T> MapAsync<T>(User user, CancellationToken cancellationToken = default);
    Task<ListModel<TOut>> MapAsync<TIn, TOut>(PagedList<TIn> aggregates, CancellationToken cancellationToken)
      where TIn : Aggregate
      where TOut : AggregateModel;
    Task<ListModel<T>> MapAsync<T>(PagedList<Session> sessions, CancellationToken cancellationToken)
      where T : AggregateModel;
  }
}
