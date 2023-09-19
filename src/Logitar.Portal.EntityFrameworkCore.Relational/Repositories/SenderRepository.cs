using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class SenderRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ISenderRepository
{
  private static readonly string AggregateType = typeof(SenderAggregate).GetName();

  private readonly ISqlHelper _sqlHelper;

  public SenderRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<SenderAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
    => await LoadAsync(new AggregateId(id), version: null, cancellationToken);
  public async Task<SenderAggregate?> LoadAsync(AggregateId id, long? version, CancellationToken cancellationToken)
    => await base.LoadAsync<SenderAggregate>(id, version, cancellationToken);

  public async Task<IEnumerable<SenderAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.Id.Value;

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Senders.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(Db.Senders.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<SenderAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task<SenderAggregate?> LoadDefaultAsync(string? tenantId, CancellationToken cancellationToken)
  {
    tenantId = tenantId?.CleanTrim();

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Senders.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(Db.Senders.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .Where(Db.Senders.IsDefault, Operators.IsEqualTo(true))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<SenderAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(SenderAggregate sender, CancellationToken cancellationToken)
    => await base.SaveAsync(sender, cancellationToken);
  public async Task SaveAsync(IEnumerable<SenderAggregate> senders, CancellationToken cancellationToken)
    => await base.SaveAsync(senders, cancellationToken);
}
