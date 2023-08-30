using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class SessionRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ISessionRepository
{
  private static readonly string AggregateType = typeof(SessionAggregate).GetName();

  private readonly ISqlHelper _sqlHelper;

  public SessionRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<SessionAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
    => await base.LoadAsync<SessionAggregate>(new AggregateId(id), cancellationToken);
  public async Task<SessionAggregate?> LoadAsync(AggregateId id, CancellationToken cancellationToken)
    => await base.LoadAsync<SessionAggregate>(id, cancellationToken);

  public async Task<IEnumerable<SessionAggregate>> LoadAsync(RealmAggregate realm, CancellationToken cancellationToken)
  {
    string tenantId = realm.Id.Value;

    IQueryBuilder query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Sessions.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(Db.Users.UserId, Db.Sessions.UserId)
      .Where(Db.Users.TenantId, Operators.IsEqualTo(tenantId))
      .SelectAll(Db.Events.Table);

    EventEntity[] events = await EventContext.Events.FromQuery(query.Build())
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<SessionAggregate>(events.Select(EventSerializer.Deserialize));
  }
  public async Task<IEnumerable<SessionAggregate>> LoadAsync(UserAggregate user, CancellationToken cancellationToken)
    => await LoadAsync(user, isActive: null, cancellationToken);
  public async Task<IEnumerable<SessionAggregate>> LoadAsync(UserAggregate user, bool? isActive, CancellationToken cancellationToken)
  {
    string aggregateId = user.Id.Value;

    IQueryBuilder query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Sessions.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(Db.Users.UserId, Db.Sessions.UserId)
      .Where(Db.Users.AggregateId, Operators.IsEqualTo(aggregateId))
      .SelectAll(Db.Events.Table);

    if (isActive.HasValue)
    {
      query = query.Where(Db.Sessions.IsActive, Operators.IsEqualTo(isActive.Value));
    }

    EventEntity[] events = await EventContext.Events.FromQuery(query.Build())
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<SessionAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task SaveAsync(SessionAggregate session, CancellationToken cancellationToken)
    => await base.SaveAsync(session, cancellationToken);
  public async Task SaveAsync(IEnumerable<SessionAggregate> sessions, CancellationToken cancellationToken)
    => await base.SaveAsync(sessions, cancellationToken);
}
