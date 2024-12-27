using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;
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
  private readonly IQueryHelper _queryHelper;

  public RealmQuerier(IActorService actorService, PortalContext context, IQueryHelper queryHelper)
  {
    _actorService = actorService;
    _realms = context.Realms;
    _queryHelper = queryHelper;
  }

  public async Task<RealmModel> ReadAsync(Realm realm, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The realm entity 'StreamId={realm.Id.Value}' could not be found.");
  }
  public async Task<RealmModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new RealmId(id), cancellationToken);
  }
  public async Task<RealmModel?> ReadAsync(RealmId id, CancellationToken cancellationToken)
  {
    string streamId = id.Value;

    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    return realm == null ? null : await MapAsync(realm, cancellationToken);
  }

  public async Task<RealmModel?> ReadAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = Helper.Normalize(uniqueSlug);

    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueSlugNormalized == uniqueSlugNormalized, cancellationToken);

    return realm == null ? null : await MapAsync(realm, cancellationToken);
  }

  public async Task<SearchResults<RealmModel>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.From(PortalDb.Realms.Table).SelectAll(PortalDb.Realms.Table)
      .ApplyIdFilter(PortalDb.Realms.StreamId, payload.Ids);
    _queryHelper.ApplyTextSearch(builder, payload.Search, PortalDb.Realms.UniqueSlug, PortalDb.Realms.DisplayName);

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
    IReadOnlyCollection<RealmModel> items = await MapAsync(realms, cancellationToken);

    return new SearchResults<RealmModel>(items, total);
  }

  private async Task<RealmModel> MapAsync(RealmEntity realm, CancellationToken cancellationToken = default)
    => (await MapAsync([realm], cancellationToken)).Single();
  private async Task<IReadOnlyCollection<RealmModel>> MapAsync(IEnumerable<RealmEntity> realms, CancellationToken cancellationToken = default)
  {
    IReadOnlyCollection<ActorId> actorIds = realms.SelectMany(user => user.GetActorIds()).ToArray();
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return realms.Select(mapper.ToRealm).ToArray();
  }
}
