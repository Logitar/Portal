using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Domain.Realms;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class RealmRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IRealmRepository
{
  private static readonly string AggregateType = typeof(RealmAggregate).GetName();

  private readonly ISqlHelper _sqlHelper;

  public RealmRepository(IEventBus eventBus, EventContext eventContext,
    IEventSerializer eventSerializer, ISqlHelper sqlHelper)
      : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<RealmAggregate?> FindAsync(string idOrUniqueSlug, CancellationToken cancellationToken)
  {
    string aggregateId = idOrUniqueSlug.Trim();
    string uniqueSlugNormalized = aggregateId.ToUpper();

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(PortalDb.Realms.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .WhereOr(
        new OperatorCondition(PortalDb.Realms.AggregateId, Operators.IsEqualTo(aggregateId)),
        new OperatorCondition(PortalDb.Realms.UniqueSlugNormalized, Operators.IsEqualTo(uniqueSlugNormalized))
      )
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    IEnumerable<RealmAggregate> realms = Load<RealmAggregate>(events.Select(EventSerializer.Deserialize));

    return realms.Count() > 1
      ? realms.FirstOrDefault(realm => realm.Id.Value == aggregateId)
      : realms.SingleOrDefault();
  }

  public async Task<RealmAggregate?> LoadAsync(AggregateId id, CancellationToken cancellationToken)
    => await base.LoadAsync<RealmAggregate>(id, cancellationToken);
  public async Task<RealmAggregate?> LoadAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = uniqueSlug.Trim().ToUpper();

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(PortalDb.Realms.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(PortalDb.Realms.UniqueSlugNormalized, Operators.IsEqualTo(uniqueSlugNormalized))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<RealmAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(RealmAggregate realm, CancellationToken cancellationToken)
    => await base.SaveAsync(realm, cancellationToken);
}
