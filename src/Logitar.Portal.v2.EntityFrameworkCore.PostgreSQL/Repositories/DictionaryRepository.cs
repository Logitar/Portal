using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Portal.v2.Core.Dictionaries;
using Logitar.Portal.v2.Core.Realms;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Repositories;

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

  public async Task<IEnumerable<DictionaryAggregate>> LoadAsync(RealmAggregate realm, CancellationToken cancellationToken)
  {
    string aggregateId = realm.Id.Value;

    EventEntity[] events = await Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Dictionaries"" d ON d.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = d.""RealmId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""AggregateId"" = {aggregateId}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<DictionaryAggregate>(events);
  }

  public async Task<DictionaryAggregate?> LoadAsync(RealmAggregate realm, CultureInfo locale, CancellationToken cancellationToken)
  {
    string aggregateId = realm.Id.Value;

    EventEntity[] events = await Context.Events.FromSqlInterpolated($@"SELECT e.* FROM ""Events"" e JOIN ""Dictionaries"" d ON d.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = d.""RealmId"" WHERE e.""AggregateType"" = {AggregateType} AND r.""AggregateId"" = {aggregateId} AND d.""Locale"" = {locale.Name}")
      .AsNoTracking()
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
