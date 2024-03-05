using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class DictionaryQuerier : IDictionaryQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<DictionaryEntity> _dictionaries;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public DictionaryQuerier(IActorService actorService, PortalContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _dictionaries = context.Dictionaries;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<Dictionary> ReadAsync(Realm? realm, DictionaryAggregate dictionary, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, dictionary.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The dictionary entity 'AggregateId={dictionary.Id.Value}' could not be found.");
  }
  public async Task<Dictionary?> ReadAsync(Realm? realm, DictionaryId id, CancellationToken cancellationToken)
    => await ReadAsync(realm, id.ToGuid(), cancellationToken);
  public async Task<Dictionary?> ReadAsync(Realm? realm, Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    DictionaryEntity? dictionary = await _dictionaries.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    if (dictionary == null || dictionary.TenantId != realm?.GetTenantId().Value)
    {
      return null;
    }

    return await MapAsync(dictionary, realm, cancellationToken);
  }

  public async Task<Dictionary?> ReadAsync(Realm? realm, string locale, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.GetTenantId().Value;
    string localeNormalized = locale.Trim().ToUpper();

    DictionaryEntity? dictionary = await _dictionaries.AsNoTracking()
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.LocaleNormalized == localeNormalized, cancellationToken);

    if (dictionary == null)
    {
      return null;
    }

    return await MapAsync(dictionary, realm, cancellationToken);
  }

  public async Task<SearchResults<Dictionary>> SearchAsync(Realm? realm, SearchDictionariesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(PortalDb.Dictionaries.Table).SelectAll(PortalDb.Dictionaries.Table)
      .ApplyRealmFilter(PortalDb.Dictionaries.TenantId, realm)
      .ApplyIdFilter(PortalDb.Dictionaries.AggregateId, payload.Ids);
    _searchHelper.ApplyTextSearch(builder, payload.Search, PortalDb.Dictionaries.Locale);

    if (payload.IsEmpty.HasValue)
    {
      ComparisonOperator @operator = payload.IsEmpty.Value ? Operators.IsEqualTo(0) : Operators.IsGreaterThan(0);
      builder.Where(PortalDb.Dictionaries.EntryCount, @operator);
    }

    IQueryable<DictionaryEntity> query = _dictionaries.FromQuery(builder).AsNoTracking();

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
    IEnumerable<Dictionary> items = await MapAsync(dictionaries, realm, cancellationToken);

    return new SearchResults<Dictionary>(items, total);
  }

  private async Task<Dictionary> MapAsync(DictionaryEntity dictionary, Realm? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([dictionary], realm, cancellationToken)).Single();
  private async Task<IEnumerable<Dictionary>> MapAsync(IEnumerable<DictionaryEntity> dictionaries, Realm? realm, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = dictionaries.SelectMany(dictionary => dictionary.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return dictionaries.Select(dictionary => mapper.ToDictionary(dictionary, realm));
  }
}
