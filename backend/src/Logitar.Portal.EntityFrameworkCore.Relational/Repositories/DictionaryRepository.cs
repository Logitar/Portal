using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Domain.Dictionaries;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class DictionaryRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IDictionaryRepository
{
  private static readonly string AggregateType = typeof(DictionaryAggregate).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public DictionaryRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<DictionaryAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
    => await base.LoadAsync<DictionaryAggregate>(new AggregateId(id), cancellationToken);

  public async Task<DictionaryAggregate?> LoadAsync(DictionaryId id, long? version, CancellationToken cancellationToken)
    => await base.LoadAsync<DictionaryAggregate>(id.AggregateId, version, cancellationToken);

  public async Task<IEnumerable<DictionaryAggregate>> LoadAsync(CancellationToken cancellationToken)
    => await base.LoadAsync<DictionaryAggregate>(cancellationToken);

  public async Task<IEnumerable<DictionaryAggregate>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(PortalDb.Dictionaries.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(PortalDb.Dictionaries.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId.Value))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<DictionaryAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task<DictionaryAggregate?> LoadAsync(TenantId? tenantId, Locale locale, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(PortalDb.Dictionaries.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(PortalDb.Dictionaries.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId.Value))
      .Where(PortalDb.Dictionaries.Locale, Operators.IsEqualTo(locale.Code))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<DictionaryAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(DictionaryAggregate dictionary, CancellationToken cancellationToken)
    => await base.SaveAsync(dictionary, cancellationToken);
  public async Task SaveAsync(IEnumerable<DictionaryAggregate> dictionaries, CancellationToken cancellationToken)
    => await base.SaveAsync(dictionaries, cancellationToken);
}
