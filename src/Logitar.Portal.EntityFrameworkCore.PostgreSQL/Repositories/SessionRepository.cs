using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Portal.Core.Caching;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Users;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Repositories;

internal class SessionRepository : EventStore, ISessionRepository
{
  private static string AggregateType { get; } = typeof(SessionAggregate).GetName();

  private readonly ICacheService _cacheService;

  public SessionRepository(ICacheService cacheService,
    EventContext context,
    IEventBus eventBus) : base(context, eventBus)
  {
    _cacheService = cacheService;
  }

  public async Task<IEnumerable<SessionAggregate>> LoadActiveAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    string aggregateId = user.Id.Value;

    EventEntity[] events = await Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Sessions"" s on s.""AggregateId"" = e.""AggregateId"" JOIN ""Users"" u on u.""UserId"" = s.""UserId"" WHERE e.""AggregateType"" = {AggregateType} AND u.""AggregateId"" = {aggregateId} AND s.""IsActive"" = true")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<SessionAggregate>(events);
  }

  public async Task<SessionAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await LoadAsync<SessionAggregate>(new AggregateId(id), cancellationToken);
  }

  public async Task<IEnumerable<SessionAggregate>> LoadAsync(RealmAggregate realm, CancellationToken cancellationToken)
  {
    string aggregateId = realm.Id.Value;

    EventEntity[] events = await Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Sessions"" s on s.""AggregateId"" = e.""AggregateId"" JOIN ""Users"" u on u.""UserId"" = s.""UserId"" JOIN ""Realms"" r ON r.""RealmId"" = u.""RealmId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""AggregateId"" = {aggregateId}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<SessionAggregate>(events);
  }

  public async Task<IEnumerable<SessionAggregate>> LoadAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    string aggregateId = user.Id.Value;

    EventEntity[] events = await Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Sessions"" s on s.""AggregateId"" = e.""AggregateId"" JOIN ""Users"" u on u.""UserId"" = s.""UserId"" WHERE e.""AggregateType"" = {AggregateType} AND u.""AggregateId"" = {aggregateId}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<SessionAggregate>(events);
  }

  public async Task SaveAsync(SessionAggregate session, CancellationToken cancellationToken)
  {
    if (session.HasChanges)
    {
      _cacheService.RemoveSession(session);
    }

    await base.SaveAsync(session, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<SessionAggregate> sessions, CancellationToken cancellationToken)
  {
    foreach (SessionAggregate session in sessions)
    {
      if (session.HasChanges)
      {
        _cacheService.RemoveSession(session);
      }
    }

    await base.SaveAsync(sessions, cancellationToken);
  }
}
