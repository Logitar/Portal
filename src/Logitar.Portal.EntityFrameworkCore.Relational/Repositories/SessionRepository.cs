using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Domain.Sessions;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class SessionRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ISessionRepository
{
  public SessionRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<SessionAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
    => await base.LoadAsync<SessionAggregate>(new AggregateId(id), cancellationToken);
  public async Task<SessionAggregate?> LoadAsync(AggregateId id, CancellationToken cancellationToken)
    => await base.LoadAsync<SessionAggregate>(id, cancellationToken);

  public async Task SaveAsync(SessionAggregate session, CancellationToken cancellationToken)
    => await base.SaveAsync(session, cancellationToken);
}
