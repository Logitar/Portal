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
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<DictionaryEntity> _dictionaries;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public DictionaryQuerier(IActorService actorService, IApplicationContext applicationContext,
    PortalContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _dictionaries = context.Dictionaries;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<Dictionary> ReadAsync(DictionaryAggregate dictionary, CancellationToken cancellationToken)
  {
    return await ReadAsync(dictionary.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The dictionary entity 'AggregateId={dictionary.Id.Value}' could not be found.");
  }
  public async Task<Dictionary?> ReadAsync(DictionaryId id, CancellationToken cancellationToken)
    => await ReadAsync(id.AggregateId.ToGuid(), cancellationToken);
  public async Task<Dictionary?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    DictionaryEntity? dictionary = await _dictionaries.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    Realm? realm = _applicationContext.Realm;
    if (dictionary == null || dictionary.TenantId != _applicationContext.TenantId?.Value)
    {
      return null;
    }

    return await MapAsync(dictionary, realm, cancellationToken);
  }

  public async Task<Dictionary?> ReadAsync(string locale, CancellationToken cancellationToken)
  {
    string? tenantId = _applicationContext.TenantId?.Value;
    locale = locale.Trim();

    DictionaryEntity? dictionary = await _dictionaries.AsNoTracking()
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Locale == locale, cancellationToken);

    if (dictionary == null)
    {
      return null;
    }

    return await MapAsync(dictionary, _applicationContext.Realm, cancellationToken);
  }

  public async Task<SearchResults<Dictionary>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken)
  {
    Realm? realm = _applicationContext.Realm;

    IQueryBuilder builder = _sqlHelper.QueryFrom(PortalDb.Dictionaries.Table).SelectAll(PortalDb.Dictionaries.Table)
      .ApplyRealmFilter(PortalDb.Dictionaries.TenantId, realm)
      .ApplyIdFilter(PortalDb.Dictionaries.AggregateId, payload.Ids);
    _searchHelper.ApplyTextSearch(builder, payload.Search, PortalDb.Dictionaries.Locale);

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
