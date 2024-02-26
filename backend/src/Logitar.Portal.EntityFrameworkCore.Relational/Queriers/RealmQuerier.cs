using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class RealmQuerier : IRealmQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<RealmEntity> _realms;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public RealmQuerier(IActorService actorService, PortalContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _realms = context.Realms;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<Realm> ReadAsync(RealmAggregate realm, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The realm entity 'AggregateId={realm.Id.Value}' could not be found.");
  }
  public async Task<Realm?> ReadAsync(RealmId id, CancellationToken cancellationToken)
    => await ReadAsync(id.ToGuid(), cancellationToken);
  public async Task<Realm?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return realm == null ? null : await MapAsync(realm, cancellationToken);
  }

  public async Task<Realm?> ReadAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = uniqueSlug.Trim().ToUpper();

    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueSlugNormalized == uniqueSlugNormalized, cancellationToken);

    return realm == null ? null : await MapAsync(realm, cancellationToken);
  }

  public async Task<SearchResults<Realm>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(PortalDb.Realms.Table).SelectAll(PortalDb.Realms.Table)
      .ApplyIdFilter(PortalDb.Realms.AggregateId, payload.Ids);
    _searchHelper.ApplyTextSearch(builder, payload.Search, PortalDb.Realms.UniqueSlug, PortalDb.Realms.DisplayName);

    IQueryable<RealmEntity> query = _realms.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<RealmEntity>? ordered = null;
    foreach (RealmSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case RealmSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case RealmSort.UniqueSlug:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueSlug) : query.OrderBy(x => x.UniqueSlug))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueSlug) : ordered.ThenBy(x => x.UniqueSlug));
          break;
        case RealmSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    RealmEntity[] realms = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Realm> items = await MapAsync(realms, cancellationToken);

    return new SearchResults<Realm>(items, total);
  }

  private async Task<Realm> MapAsync(RealmEntity realm, CancellationToken cancellationToken = default)
    => (await MapAsync([realm], cancellationToken)).Single();
  private async Task<IEnumerable<Realm>> MapAsync(IEnumerable<RealmEntity> realms, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = realms.SelectMany(user => user.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return realms.Select(mapper.ToRealm);
  }
}
