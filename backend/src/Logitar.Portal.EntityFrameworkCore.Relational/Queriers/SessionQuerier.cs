using Logitar.EventSourcing;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class SessionQuerier : ISessionQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<SessionEntity> _sessions;

  public SessionQuerier(IActorService actorService, IApplicationContext applicationContext, IdentityContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _sessions = context.Sessions;
  }

  public async Task<Session> ReadAsync(SessionAggregate session, CancellationToken cancellationToken)
  {
    return await ReadAsync(session.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The session entity 'AggregateId={session.Id.Value}' could not be found.");
  }
  public async Task<Session?> ReadAsync(SessionId id, CancellationToken cancellationToken)
    => await ReadAsync(id.Value, cancellationToken);
  public async Task<Session?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    string aggregateId = id.Trim();

    SessionEntity? session = await _sessions.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Identifiers)
      .Include(x => x.User).ThenInclude(x => x!.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    Realm? realm = _applicationContext.Realm;
    if (session == null || session.User == null || session.User.TenantId != realm?.Id)
    {
      return null;
    }

    return await MapAsync(session, realm, cancellationToken);
  }

  private async Task<Session> MapAsync(SessionEntity session, Realm? realm, CancellationToken cancellationToken)
    => (await MapAsync([session], realm, cancellationToken)).Single();
  private async Task<IEnumerable<Session>> MapAsync(IEnumerable<SessionEntity> sessions, Realm? realm, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = sessions.SelectMany(session => session.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return sessions.Select(session => mapper.ToSession(session, realm));
  }
}
