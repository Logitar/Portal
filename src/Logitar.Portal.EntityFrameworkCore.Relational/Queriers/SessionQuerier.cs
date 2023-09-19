using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class SessionQuerier : ISessionQuerier
{
  private readonly IActorService _actorService;
  private readonly IRealmQuerier _realmQuerier;
  private readonly DbSet<SessionEntity> _sessions;
  private readonly ISqlHelper _sqlHelper;

  public SessionQuerier(IActorService actorService, PortalContext context, IRealmQuerier realmQuerier, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _realmQuerier = realmQuerier;
    _sessions = context.Sessions;
    _sqlHelper = sqlHelper;
  }

  public async Task<Session> ReadAsync(SessionAggregate session, CancellationToken cancellationToken)
  {
    return await ReadAsync(session.Id, cancellationToken)
      ?? throw new EntityNotFoundException<SessionEntity>(session.Id);
  }
  public async Task<Session?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new AggregateId(id), cancellationToken);
  }
  private async Task<Session?> ReadAsync(AggregateId id, CancellationToken cancellationToken)
  {
    string aggregateId = id.Value;

    SessionEntity? session = await _sessions.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);
    if (session == null)
    {
      return null;
    }

    Realm? realm = null;
    if (session.User?.TenantId != null)
    {
      AggregateId realmId = new(session.User.TenantId);
      realm = await _realmQuerier.ReadAsync(realmId, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(realmId);
    }

    return (await MapAsync(realm, cancellationToken, session)).Single();
  }

  public async Task<SearchResults<Session>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken)
  {
    Realm? realm = null;
    string? tenantId = null;
    if (!string.IsNullOrWhiteSpace(payload.Realm))
    {
      realm = await _realmQuerier.FindAsync(payload.Realm, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(payload.Realm);
      tenantId = new AggregateId(realm.Id).Value;
    }

    IQueryBuilder builder = _sqlHelper.QueryFrom(Db.Sessions.Table)
      .Join(Db.Users.UserId, Db.Sessions.UserId)
      .ApplyIdInFilter(Db.Sessions.AggregateId, payload.IdIn)
      .Where(Db.Users.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .SelectAll(Db.Sessions.Table);
    _sqlHelper.ApplyTextSearch(builder, payload.Search);

    if (payload.UserId.HasValue)
    {
      string aggregateId = new AggregateId(payload.UserId.Value).Value;
      builder = builder.Where(Db.Users.AggregateId, Operators.IsEqualTo(aggregateId));
    }

    if (payload.IsActive.HasValue)
    {
      builder = builder.Where(Db.Sessions.IsActive, Operators.IsEqualTo(payload.IsActive.Value));
    }
    if (payload.IsPersistent.HasValue)
    {
      builder = builder.Where(Db.Sessions.IsPersistent, Operators.IsEqualTo(payload.IsPersistent.Value));
    }

    IQueryable<SessionEntity> query = _sessions.FromQuery(builder.Build())
      .AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Roles);
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<SessionEntity>? ordered = null;
    foreach (SessionSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case SessionSort.SignedOutOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.SignedOutOn) : query.OrderBy(x => x.SignedOutOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.SignedOutOn) : ordered.ThenBy(x => x.SignedOutOn));
          break;
        case SessionSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    SessionEntity[] sessions = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Session> results = await MapAsync(realm, cancellationToken, sessions);

    return new SearchResults<Session>(results, total);
  }

  private async Task<IEnumerable<Session>> MapAsync(Realm? realm = null, CancellationToken cancellationToken = default, params SessionEntity[] sessions)
  {
    IEnumerable<ActorId> actorIds = sessions.SelectMany(session => session.GetActorIds()).Distinct();
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return sessions.Select(session => mapper.ToSession(session, realm));
  }
}
