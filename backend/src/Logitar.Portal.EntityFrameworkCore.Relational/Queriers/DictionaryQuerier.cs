using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;
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
  private readonly IQueryHelper _queryHelper;

  public DictionaryQuerier(IActorService actorService, PortalContext context, IQueryHelper queryHelper)
  {
    _actorService = actorService;
    _dictionaries = context.Dictionaries;
    _queryHelper = queryHelper;
  }

  public async Task<DictionaryModel> ReadAsync(RealmModel? realm, Dictionary dictionary, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, dictionary.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The dictionary entity 'StreamId={dictionary.Id.Value}' could not be found.");
  }
  public async Task<DictionaryModel?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, new DictionaryId(realm?.GetTenantId(), new EntityId(id)), cancellationToken);
  }
  public async Task<DictionaryModel?> ReadAsync(RealmModel? realm, DictionaryId id, CancellationToken cancellationToken)
  {
    string streamId = id.Value;

    DictionaryEntity? dictionary = await _dictionaries.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (dictionary == null || dictionary.TenantId != realm?.GetTenantId().ToGuid())
    {
      return null;
    }

    return await MapAsync(dictionary, realm, cancellationToken);
  }

  public async Task<DictionaryModel?> ReadAsync(RealmModel? realm, string locale, CancellationToken cancellationToken)
  {
    Guid? tenantId = realm?.GetTenantId().ToGuid();
    string localeNormalized = Helper.Normalize(locale);

    DictionaryEntity? dictionary = await _dictionaries.AsNoTracking()
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.LocaleNormalized == localeNormalized, cancellationToken);

    if (dictionary == null)
    {
      return null;
    }

    return await MapAsync(dictionary, realm, cancellationToken);
  }

  public async Task<SearchResults<DictionaryModel>> SearchAsync(RealmModel? realm, SearchDictionariesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.QueryFrom(PortalDb.Dictionaries.Table).SelectAll(PortalDb.Dictionaries.Table)
      .ApplyRealmFilter(PortalDb.Dictionaries.TenantId, realm)
      .ApplyIdFilter(PortalDb.Dictionaries.StreamId, payload.Ids);
    _queryHelper.ApplyTextSearch(builder, payload.Search, PortalDb.Dictionaries.Locale);

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
    IEnumerable<DictionaryModel> items = await MapAsync(dictionaries, realm, cancellationToken);

    return new SearchResults<DictionaryModel>(items, total);
  }

  private async Task<DictionaryModel> MapAsync(DictionaryEntity dictionary, RealmModel? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([dictionary], realm, cancellationToken)).Single();
  private async Task<IEnumerable<DictionaryModel>> MapAsync(IEnumerable<DictionaryEntity> dictionaries, RealmModel? realm, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = dictionaries.SelectMany(dictionary => dictionary.GetActorIds());
    IEnumerable<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return dictionaries.Select(dictionary => mapper.ToDictionary(dictionary, realm));
  }
}
