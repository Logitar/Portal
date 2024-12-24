using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;
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
  private readonly IQueryHelper _queryHelper;
  private readonly DbSet<SessionEntity> _sessions;

  public SessionQuerier(IActorService actorService, IdentityContext context, IQueryHelper queryHelper)
  {
    _actorService = actorService;
    _queryHelper = queryHelper;
    _sessions = context.Sessions;
  }

  public async Task<SessionModel> ReadAsync(RealmModel? realm, Session session, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, session.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The session entity 'StreamId={session.Id}' could not be found.");
  }
  public async Task<SessionModel?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, new SessionId(realm?.GetTenantId(), new EntityId(id)), cancellationToken);
  }
  public async Task<SessionModel?> ReadAsync(RealmModel? realm, SessionId id, CancellationToken cancellationToken)
  {
    string streamId = id.Value;

    SessionEntity? session = await _sessions.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Identifiers)
      .Include(x => x.User).ThenInclude(x => x!.Roles)
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (session == null || session.User == null || session.User.TenantId != realm?.GetTenantId().Value)
    {
      return null;
    }

    return await MapAsync(session, realm, cancellationToken);
  }

  public async Task<SearchResults<SessionModel>> SearchAsync(RealmModel? realm, SearchSessionsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.QueryFrom(Sessions.Table).SelectAll(Sessions.Table)
      .Join(Users.UserId, Sessions.UserId)
      .ApplyRealmFilter(Users.TenantId, realm)
      .ApplyIdFilter(Sessions.StreamId, payload.Ids);
    _queryHelper.ApplyTextSearch(builder, payload.Search);

    if (payload.UserId.HasValue)
    {
      UserId userId = new(new StreamId(payload.UserId.Value));
      builder.Where(Users.StreamId, Operators.IsEqualTo(userId.Value));
    }
    if (payload.IsActive.HasValue)
    {
      builder.Where(Sessions.IsActive, Operators.IsEqualTo(payload.IsActive.Value));
    }
    if (payload.IsPersistent.HasValue)
    {
      builder.Where(Sessions.IsPersistent, Operators.IsEqualTo(payload.IsPersistent.Value));
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
    IEnumerable<SessionModel> items = await MapAsync(sessions, realm, cancellationToken);

    return new SearchResults<SessionModel>(items, total);
  }

  private async Task<SessionModel> MapAsync(SessionEntity session, RealmModel? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([session], realm, cancellationToken)).Single();
  private async Task<IEnumerable<SessionModel>> MapAsync(IEnumerable<SessionEntity> sessions, RealmModel? realm, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = sessions.SelectMany(session => session.GetActorIds());
    IEnumerable<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return sessions.Select(session => mapper.ToSession(session, realm));
  }
}
