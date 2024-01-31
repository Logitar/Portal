using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class SessionQuerier : ISessionQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly ISearchHelper _searchHelper;
  private readonly DbSet<SessionEntity> _sessions;
  private readonly ISqlHelper _sqlHelper;

  public SessionQuerier(IActorService actorService, IApplicationContext applicationContext,
    IdentityContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _sessions = context.Sessions;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<Session> ReadAsync(SessionAggregate session, CancellationToken cancellationToken)
  {
    return await ReadAsync(session.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The session entity 'AggregateId={session.Id.Value}' could not be found.");
  }
  public async Task<Session?> ReadAsync(SessionId id, CancellationToken cancellationToken)
    => await ReadAsync(id.AggregateId.ToGuid(), cancellationToken);
  public async Task<Session?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    SessionEntity? session = await _sessions.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Identifiers)
      .Include(x => x.User).ThenInclude(x => x!.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    Realm? realm = _applicationContext.Realm;
    if (session == null || session.User == null || session.User.TenantId != _applicationContext.TenantId?.Value)
    {
      return null;
    }

    return await MapAsync(session, realm, cancellationToken);
  }

  public async Task<SearchResults<Session>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken)
  {
    Realm? realm = _applicationContext.Realm;

    IQueryBuilder builder = _sqlHelper.QueryFrom(IdentityDb.Sessions.Table).SelectAll(IdentityDb.Sessions.Table)
      .Join(IdentityDb.Users.UserId, IdentityDb.Sessions.UserId)
      .ApplyRealmFilter(IdentityDb.Users.TenantId, realm)
      .ApplyIdFilter(IdentityDb.Sessions.AggregateId, payload.Ids);
    _searchHelper.ApplyTextSearch(builder, payload.Search);

    if (payload.UserId.HasValue)
    {
      UserId userId = new(new AggregateId(payload.UserId.Value).Value);
      builder.Where(IdentityDb.Users.AggregateId, Operators.IsEqualTo(userId.Value));
    }
    if (payload.IsActive.HasValue)
    {
      builder.Where(IdentityDb.Sessions.IsActive, Operators.IsEqualTo(payload.IsActive.Value));
    }
    if (payload.IsPersistent.HasValue)
    {
      builder.Where(IdentityDb.Sessions.IsPersistent, Operators.IsEqualTo(payload.IsPersistent.Value));
    }

    IQueryable<SessionEntity> query = _sessions.FromQuery(builder).AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Identifiers)
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
    IEnumerable<Session> items = await MapAsync(sessions, realm, cancellationToken);

    return new SearchResults<Session>(items, total);
  }

  private async Task<Session> MapAsync(SessionEntity session, Realm? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([session], realm, cancellationToken)).Single();
  private async Task<IEnumerable<Session>> MapAsync(IEnumerable<SessionEntity> sessions, Realm? realm, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = sessions.SelectMany(session => session.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return sessions.Select(session => mapper.ToSession(session, realm));
  }
}
