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
  private readonly ISqlHelper _sql;

  public RealmRepository(IEventBus eventBus, EventContext eventContext,
    IEventSerializer eventSerializer, ISqlHelper sql) : base(eventBus, eventContext, eventSerializer)
  {
    _sql = sql;
  }

  protected string AggregateType { get; } = typeof(RealmAggregate).GetName();

  public async Task<RealmAggregate?> LoadAsync(AggregateId id, CancellationToken cancellationToken)
    => await LoadAsync(id, version: null, cancellationToken);
  public async Task<RealmAggregate?> LoadAsync(AggregateId id, long? version, CancellationToken cancellationToken)
    => await base.LoadAsync<RealmAggregate>(id, version, cancellationToken);

  public async Task<RealmAggregate?> LoadAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = uniqueSlug.ToUpper();

    IQuery query = _sql.QueryFrom(Db.Events.Table)
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

    return base.Load<RealmAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(RealmAggregate realm, CancellationToken cancellationToken)
    => await base.SaveAsync(realm, cancellationToken);
  public async Task SaveAsync(IEnumerable<RealmAggregate> realms, CancellationToken cancellationToken)
    => await base.SaveAsync(realms, cancellationToken);
}
