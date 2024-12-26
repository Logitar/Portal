using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Domain.Senders;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class SenderRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ISenderRepository
{
  private static readonly string AggregateType = typeof(Sender).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public SenderRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<Sender?> LoadAsync(Guid id, CancellationToken cancellationToken)
    => await LoadAsync<Sender>(new AggregateId(id), cancellationToken);

  public async Task<Sender?> LoadAsync(SenderId id, CancellationToken cancellationToken)
    => await LoadAsync(id, version: null, cancellationToken);
  public async Task<Sender?> LoadAsync(SenderId id, long? version, CancellationToken cancellationToken)
    => await LoadAsync<Sender>(id.AggregateId, version, cancellationToken);

  public async Task<IEnumerable<Sender>> LoadAsync(CancellationToken cancellationToken)
    => await LoadAsync<Sender>(cancellationToken);

  public async Task<IEnumerable<Sender>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(PortalDb.Senders.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(PortalDb.Senders.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId.Value))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<Sender>(events.Select(EventSerializer.Deserialize));
  }

  public async Task<Sender?> LoadDefaultAsync(TenantId? tenantId, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(PortalDb.Senders.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(PortalDb.Senders.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId.Value))
      .Where(PortalDb.Senders.IsDefault, Operators.IsEqualTo(true))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<Sender>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(Sender sender, CancellationToken cancellationToken)
    => await base.SaveAsync(sender, cancellationToken);
  public async Task SaveAsync(IEnumerable<Sender> senders, CancellationToken cancellationToken)
    => await base.SaveAsync(senders, cancellationToken);
}
