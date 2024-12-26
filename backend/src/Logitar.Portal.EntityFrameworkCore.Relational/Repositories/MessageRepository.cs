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
  private static readonly string AggregateType = typeof(Message).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public MessageRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<Message?> LoadAsync(Guid id, CancellationToken cancellationToken)
    => await base.LoadAsync<Message>(new AggregateId(id), cancellationToken);

  public async Task<IReadOnlyCollection<Message>> LoadAsync(CancellationToken cancellationToken)
    => (await base.LoadAsync<Message>(cancellationToken)).ToArray(); // ISSUE #528: remove ToArray

  public async Task<IReadOnlyCollection<Message>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken)
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

    return (Load<Message>(events.Select(EventSerializer.Deserialize))).ToArray(); // ISSUE #528: remove ToArray
  }

  public async Task SaveAsync(Message message, CancellationToken cancellationToken)
    => await base.SaveAsync(message, cancellationToken);

  public async Task SaveAsync(IEnumerable<Message> messages, CancellationToken cancellationToken)
    => await base.SaveAsync(messages, cancellationToken);
}
