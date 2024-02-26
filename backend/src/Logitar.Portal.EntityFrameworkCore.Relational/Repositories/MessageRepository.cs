using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Domain.Messages;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class MessageRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IMessageRepository
{
  private static readonly string AggregateType = typeof(MessageAggregate).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public MessageRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<MessageAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
    => await base.LoadAsync<MessageAggregate>(new AggregateId(id), cancellationToken);

  public async Task<IEnumerable<MessageAggregate>> LoadAsync(CancellationToken cancellationToken)
    => await base.LoadAsync<MessageAggregate>(cancellationToken);

  public async Task<IEnumerable<MessageAggregate>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(PortalDb.Messages.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(PortalDb.Messages.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId.Value))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<MessageAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task SaveAsync(MessageAggregate message, CancellationToken cancellationToken)
    => await base.SaveAsync(message, cancellationToken);

  public async Task SaveAsync(IEnumerable<MessageAggregate> messages, CancellationToken cancellationToken)
    => await base.SaveAsync(messages, cancellationToken);
}
