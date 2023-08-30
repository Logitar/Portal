using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Application;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class RealmRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IRealmRepository
{
  private static readonly string AggregateType = typeof(RealmAggregate).GetName();

  private readonly ISqlHelper _sqlHelper;

  public RealmRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<RealmAggregate?> FindAsync(string idOrUniqueSlug, CancellationToken cancellationToken)
  {
    idOrUniqueSlug = idOrUniqueSlug.Trim();
    string uniqueSlugNormalized = idOrUniqueSlug.ToUpper();

    IQueryBuilder query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Realms.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .SelectAll(Db.Events.Table);

    List<Condition> conditions = new(capacity: 2)
    {
      new OperatorCondition(Db.Realms.UniqueSlugNormalized, Operators.IsEqualTo(uniqueSlugNormalized))
    };
    if (Guid.TryParse(idOrUniqueSlug, out Guid id))
    {
      string aggregateId = new AggregateId(id).Value;
      conditions.Add(new OperatorCondition(Db.Realms.AggregateId, Operators.IsEqualTo(aggregateId)));
    }
    query = conditions.Count == 1 ? query.Where(conditions.Single()) : query.WhereOr(conditions.ToArray());

    EventEntity[] events = await EventContext.Events.FromQuery(query.Build())
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    IEnumerable<RealmAggregate> realms = Load<RealmAggregate>(events.Select(EventSerializer.Deserialize));
    if (realms.Count() > 1)
    {
      return realms.First(realm => realm.Id.Value == idOrUniqueSlug);
    }

    return realms.SingleOrDefault();
  }

  public async Task<RealmAggregate?> LoadAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = uniqueSlug.Trim().ToUpper();

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Realms.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(Db.Realms.UniqueSlugNormalized, Operators.IsEqualTo(uniqueSlugNormalized))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<RealmAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task<RealmAggregate?> LoadAsync(SessionAggregate session, CancellationToken cancellationToken)
  {
    UserAggregate user = await LoadAsync<UserAggregate>(session.UserId, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(session.UserId, $"{nameof(session)}.{nameof(session.UserId)}");

    return await LoadAsync(user, cancellationToken);
  }

  public async Task<RealmAggregate?> LoadAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    if (user.TenantId == null)
    {
      return null;
    }

    AggregateId id = new(user.TenantId);

    return await base.LoadAsync<RealmAggregate>(id, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(id, $"{nameof(user)}.{nameof(user.TenantId)}");
  }

  public async Task SaveAsync(RealmAggregate realm, CancellationToken cancellationToken)
    => await base.SaveAsync(realm, cancellationToken);
}
