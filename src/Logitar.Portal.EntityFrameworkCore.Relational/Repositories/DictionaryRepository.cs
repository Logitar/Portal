using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Realms;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class DictionaryRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IDictionaryRepository
{
  private static readonly string AggregateType = typeof(DictionaryAggregate).GetName();

  private readonly ISqlHelper _sqlHelper;

  public DictionaryRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<DictionaryAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
    => await LoadAsync(new AggregateId(id), version: null, cancellationToken);
  public async Task<DictionaryAggregate?> LoadAsync(AggregateId id, long? version, CancellationToken cancellationToken)
    => await LoadAsync<DictionaryAggregate>(id, version, cancellationToken);

  public async Task<DictionaryAggregate?> LoadAsync(string? tenantId, ReadOnlyLocale locale, CancellationToken cancellationToken)
  {
    tenantId = tenantId?.CleanTrim();
    string localeCode = locale.Code;

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Dictionaries.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(Db.Dictionaries.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .Where(Db.Dictionaries.Locale, Operators.IsEqualTo(localeCode))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<DictionaryAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task<IEnumerable<DictionaryAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.Id.Value;

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Dictionaries.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(Db.Dictionaries.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<DictionaryAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task SaveAsync(DictionaryAggregate dictionary, CancellationToken cancellationToken)
    => await base.SaveAsync(dictionary, cancellationToken);
  public async Task SaveAsync(IEnumerable<DictionaryAggregate> dictionaries, CancellationToken cancellationToken)
    => await base.SaveAsync(dictionaries, cancellationToken);
}
