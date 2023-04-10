using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.v2.Core.Sessions;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Repositories;

internal class SessionRepository : EventStore, ISessionRepository
{
  public SessionRepository(EventContext context, IEventBus eventBus) : base(context, eventBus)
  {
  }

  public async Task<SessionAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await LoadAsync<SessionAggregate>(new AggregateId(id), cancellationToken);
  }

  public async Task SaveAsync(SessionAggregate session, CancellationToken cancellationToken)
  {
    await base.SaveAsync(session, cancellationToken);
  }
}
