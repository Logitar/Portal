using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Realms;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class MessageRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IMessageRepository
{
  private static readonly string AggregateType = typeof(MessageAggregate).GetName();

  private readonly ISqlHelper _sqlHelper;

  public MessageRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<IEnumerable<MessageAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Messages.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(new Join(JoinKind.Left, Db.Realms.RealmId, Db.Messages.RealmId))
      .SelectAll(Db.Events.Table);

    if (realm == null)
    {
      builder = builder.Where(Db.Messages.RealmId, Operators.IsNull());
    }
    else
    {
      string aggregateId = realm.Id.Value;
      builder = builder.Where(Db.Realms.AggregateId, Operators.IsEqualTo(aggregateId));
    }

    EventEntity[] events = await EventContext.Events.FromQuery(builder.Build())
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<MessageAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task SaveAsync(MessageAggregate message, CancellationToken cancellationToken)
    => await base.SaveAsync(message, cancellationToken);
  public async Task SaveAsync(IEnumerable<MessageAggregate> messages, CancellationToken cancellationToken)
    => await base.SaveAsync(messages, cancellationToken);
}
