using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class SessionQuerier : ISessionQuerier
{
  private readonly IActorService _actorService;
  private readonly IQueryHelper _queryHelper;
  private readonly DbSet<RealmEntity> _realms;
  private readonly DbSet<SessionEntity> _sessions;

  public SessionQuerier(IActorService actorService, IdentityContext identityContext,
    IQueryHelper queryHelper, PortalContext portalContext)
  {
    _actorService = actorService;
    _queryHelper = queryHelper;
    _realms = portalContext.Realms;
    _sessions = identityContext.Sessions;
  }

  public async Task<Session> ReadAsync(SessionAggregate session, CancellationToken cancellationToken)
    => await ReadAsync(session.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The session entity 'Id={session.Id}' could not be found.");
  public async Task<Session?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    SessionEntity? session = await _sessions.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

    RealmEntity? realm = null;
    if (session?.User?.TenantId != null)
    {
      realm = await _realms.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == session.User.TenantId, cancellationToken)
        ?? throw new InvalidOperationException($"The realm 'Id={session.User.TenantId}' could not be found from user 'Id={session.User.AggregateId}'.");
    }
    return await MapAsync(session, realm, cancellationToken);
  }

  public async Task<SearchResults<Session>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken)
  {
    RealmEntity? realm = null;
    if (payload.Realm != null)
    {
      realm = await FindRealmAsync(payload.Realm, cancellationToken);
      if (realm == null)
      {
        return new SearchResults<Session>();
      }
    }
    string? tenantId = realm?.AggregateId;

    IQueryBuilder builder = _queryHelper.From(Db.Sessions.Table)
      .Join(Db.Users.UserId, Db.Sessions.UserId)
      .Where(Db.Users.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .SelectAll(Db.Sessions.Table);

    _queryHelper.ApplyTextSearch(builder, payload.Id, Db.Sessions.AggregateId);
    _queryHelper.ApplyTextSearch(builder, payload.Search);

    if (payload.UserId != null)
    {
      string aggregateId = payload.UserId.Trim();
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

    IQueryable<SessionEntity> query = _sessions.FromQuery(builder.Build()).AsNoTracking()
      .Include(x => x.User);

    long total = await query.LongCountAsync(cancellationToken);

    if (payload.Sort.Any())
    {
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

      if (ordered != null)
      {
        query = ordered;
      }
    }

    query = query.ApplyPaging(payload);

    SessionEntity[] sessions = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Session> results = await MapAsync(sessions, realm: null, cancellationToken);

    return new SearchResults<Session>(results, total);
  }

  private async Task<RealmEntity?> FindRealmAsync(string realm, CancellationToken cancellationToken)
  {
    string aggregateId = realm.Trim();
    string uniqueSlugNormalized = aggregateId.ToUpper();

    RealmEntity[] realms = await _realms.AsNoTracking()
      .Where(x => x.AggregateId == aggregateId || x.UniqueSlugNormalized == uniqueSlugNormalized)
      .ToArrayAsync(cancellationToken);

    return realms.Length > 1
      ? realms.FirstOrDefault(realm => realm.AggregateId == aggregateId)
      : realms.SingleOrDefault();
  }

  private async Task<Session?> MapAsync(SessionEntity? session, RealmEntity? realm, CancellationToken cancellationToken)
  {
    if (session == null)
    {
      return null;
    }

    return (await MapAsync(new[] { session }, realm, cancellationToken)).Single();
  }
  private async Task<IEnumerable<Session>> MapAsync(IEnumerable<SessionEntity> sessions, RealmEntity? realm, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = ActorHelper.GetIds(sessions.ToArray());
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Realm? mappedRealm = realm == null ? null : mapper.Map(realm);

    return sessions.Select(session => mapper.Map(session, mappedRealm));
  }
}
