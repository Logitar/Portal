using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Portal.Core.Dictionaries;
using Logitar.Portal.Core.Realms;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Repositories;

internal class DictionaryRepository : EventStore, IDictionaryRepository
{
  private static string AggregateType { get; } = typeof(DictionaryAggregate).GetName();

  public DictionaryRepository(EventContext context, IEventBus eventBus) : base(context, eventBus)
  {
  }

  public async Task<DictionaryAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await LoadAsync<DictionaryAggregate>(new AggregateId(id), cancellationToken);
  }

  public async Task<IEnumerable<DictionaryAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken)
  {
    IQueryable<EventEntity> query;
    if (realm == null)
    {
      query = Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Dictionaries"" d ON d.""AggregateId"" = e.""AggregateId"" WHERE e.""AggregateType"" = {AggregateType} AND d.""RealmId"" IS NULL");
    }
    else
    {
      string aggregateId = realm.Id.Value;
      query = Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Dictionaries"" d ON d.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = d.""RealmId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""AggregateId"" = {aggregateId}");
    }

    EventEntity[] events = await query.AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<DictionaryAggregate>(events);
  }

  public async Task<DictionaryAggregate?> LoadAsync(RealmAggregate? realm, CultureInfo locale, CancellationToken cancellationToken)
  {
    IQueryable<EventEntity> query;
    if (realm == null)
    {
      query = Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Dictionaries"" d ON d.""AggregateId"" = e.""AggregateId"" WHERE e.""AggregateType"" = {AggregateType} AND d.""RealmId"" IS NULL AND d.""Locale"" = {locale.Name}");
    }
    else
    {
      string aggregateId = realm.Id.Value;
      query = Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Dictionaries"" d ON d.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = d.""RealmId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""AggregateId"" = {aggregateId} AND d.""Locale"" = {locale.Name}");
    }

    EventEntity[] events = await query.AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<DictionaryAggregate>(events).SingleOrDefault();
  }

  public async Task SaveAsync(DictionaryAggregate dictionary, CancellationToken cancellationToken)
  {
    await base.SaveAsync(dictionary, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<DictionaryAggregate> dictionaries, CancellationToken cancellationToken)
  {
    await base.SaveAsync(dictionaries, cancellationToken);
  }
}
