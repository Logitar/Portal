using Logitar.EventSourcing;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class SessionQuerier : ISessionQuerier
{
  private readonly IActorService _actorService;
  private readonly IRealmQuerier _realmQuerier;
  private readonly DbSet<SessionEntity> _sessions;

  public SessionQuerier(IActorService actorService, PortalContext context, IRealmQuerier realmQuerier)
  {
    _actorService = actorService;
    _realmQuerier = realmQuerier;
    _sessions = context.Sessions;
  }

  public async Task<Session> ReadAsync(SessionAggregate session, CancellationToken cancellationToken)
  {
    return await ReadAsync(session.Id, cancellationToken)
      ?? throw new EntityNotFoundException<SessionEntity>(session.Id);
  }
  public async Task<Session?> ReadAsync(AggregateId id, CancellationToken cancellationToken)
  {
    string aggregateId = id.Value.Trim();

    SessionEntity? session = await _sessions.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);
    if (session == null)
    {
      return null;
    }

    Realm? realm = session.User?.TenantId == null ? null
      : await _realmQuerier.ReadAsync(new AggregateId(session.User.TenantId), cancellationToken);

    IEnumerable<ActorId> actorIds = new[] { session.CreatedBy, session.UpdatedBy, session.SignedOutBy }
      .Where(id => id != null)
      .Select(id => new ActorId(id!));
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return mapper.ToSession(session, realm: null);
  }
}
