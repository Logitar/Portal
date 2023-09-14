using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class DictionaryQuerier : IDictionaryQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<DictionaryEntity> _dictionaries;
  private readonly IRealmQuerier _realmQuerier;
  private readonly ISqlHelper _sqlHelper;

  public DictionaryQuerier(IActorService actorService, PortalContext context, IRealmQuerier realmQuerier, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _dictionaries = context.Dictionaries;
    _realmQuerier = realmQuerier;
    _sqlHelper = sqlHelper;
  }

  public async Task<Dictionary> ReadAsync(DictionaryAggregate dictionary, CancellationToken cancellationToken)
  {
    return await ReadAsync(dictionary.Id, cancellationToken)
      ?? throw new EntityNotFoundException<DictionaryEntity>(dictionary.Id);
  }
  public async Task<Dictionary?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new AggregateId(id), cancellationToken);
  }
  private async Task<Dictionary?> ReadAsync(AggregateId id, CancellationToken cancellationToken)
  {
    string aggregateId = id.Value;

    DictionaryEntity? dictionary = await _dictionaries.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);
    if (dictionary == null)
    {
      return null;
    }

    Realm? realm = null;
    if (dictionary.TenantId != null)
    {
      AggregateId realmId = new(dictionary.TenantId);
      realm = await _realmQuerier.ReadAsync(realmId, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(realmId);
    }

    return (await MapAsync(realm, cancellationToken, dictionary)).Single();
  }

  public async Task<Dictionary?> ReadAsync(string? realmIdOrUniqueSlug, string locale, CancellationToken cancellationToken)
  {
    Realm? realm = null;
    if (!string.IsNullOrWhiteSpace(realmIdOrUniqueSlug))
    {
      realm = await _realmQuerier.FindAsync(realmIdOrUniqueSlug, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(realmIdOrUniqueSlug);
    }

    string? tenantId = realm == null ? null : new AggregateId(realm.Id).Value;
    locale = locale.Trim();

    DictionaryEntity? dictionary = await _dictionaries.AsNoTracking()
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Locale == locale, cancellationToken);
    if (dictionary == null)
    {
      return null;
    }

    return (await MapAsync(realm, cancellationToken, dictionary)).Single();
  }

  public async Task<SearchResults<Dictionary>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken)
  {
    Realm? realm = null;
    string? tenantId = null;
    if (!string.IsNullOrWhiteSpace(payload.Realm))
    {
      realm = await _realmQuerier.FindAsync(payload.Realm, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(payload.Realm);
      tenantId = new AggregateId(realm.Id).Value;
    }

    IQueryBuilder builder = _sqlHelper.QueryFrom(Db.Dictionaries.Table)
      .ApplyIdInFilter(Db.Dictionaries.AggregateId, payload.IdIn)
      .Where(Db.Dictionaries.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .SelectAll(Db.Dictionaries.Table);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Dictionaries.Locale);

    IQueryable<DictionaryEntity> query = _dictionaries.FromQuery(builder.Build())
      .AsNoTracking();
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<DictionaryEntity>? ordered = null;
    foreach (DictionarySortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case DictionarySort.EntryCount:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.EntryCount) : query.OrderBy(x => x.EntryCount))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.EntryCount) : ordered.ThenBy(x => x.EntryCount));
          break;
        case DictionarySort.Locale:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Locale) : query.OrderBy(x => x.Locale))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Locale) : ordered.ThenBy(x => x.Locale));
          break;
        case DictionarySort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    DictionaryEntity[] dictionaries = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Dictionary> results = await MapAsync(realm, cancellationToken, dictionaries);

    return new SearchResults<Dictionary>(results, total);
  }

  private async Task<IEnumerable<Dictionary>> MapAsync(Realm? realm = null, CancellationToken cancellationToken = default, params DictionaryEntity[] dictionaries)
  {
    IEnumerable<ActorId> actorIds = dictionaries.SelectMany(dictionary => dictionary.GetActorIds()).Distinct();
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return dictionaries.Select(dictionary => mapper.ToDictionary(dictionary, realm));
  }
}
