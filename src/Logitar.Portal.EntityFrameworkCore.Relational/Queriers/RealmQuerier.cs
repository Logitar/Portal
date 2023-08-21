using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class RealmQuerier : IRealmQuerier
{
  private readonly IActorService _actorService;
  private readonly IQueryHelper _queryHelper;
  private readonly DbSet<RealmEntity> _realms;

  public RealmQuerier(IActorService actorService, PortalContext context, IQueryHelper queryHelper)
  {
    _actorService = actorService;
    _realms = context.Realms;
    _queryHelper = queryHelper;
  }

  public async Task<Realm> ReadAsync(RealmAggregate realm, CancellationToken cancellationToken)
    => await ReadAsync(realm.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The realm entity 'Id={realm.Id}' could not be found.");
  public async Task<Realm?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

    return await MapAsync(realm, cancellationToken);
  }

  public async Task<Realm?> ReadByUniqueSlugAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = uniqueSlug.Trim().ToUpper();

    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueSlugNormalized == uniqueSlugNormalized, cancellationToken);

    return await MapAsync(realm, cancellationToken);
  }

  public async Task<SearchResults<Realm>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.From(PortalDb.Realms.Table).SelectAll(PortalDb.Realms.Table);
    _queryHelper.ApplyTextSearch(builder, payload.Id, PortalDb.Realms.AggregateId);
    _queryHelper.ApplyTextSearch(builder, payload.Search, PortalDb.Realms.UniqueSlug, PortalDb.Realms.DisplayName);

    IQueryable<RealmEntity> query = _realms.FromQuery(builder.Build()).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    int sortCount = payload.Sort.Count();
    if (sortCount > 0)
    {
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

      if (ordered != null)
      {
        query = ordered;
      }
    }

    query = query.ApplyPaging(payload);

    RealmEntity[] realms = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Realm> results = await MapAsync(realms, cancellationToken);

    return new SearchResults<Realm>(results, total);
  }

  private async Task<Realm?> MapAsync(RealmEntity? realm, CancellationToken cancellationToken)
  {
    if (realm == null)
    {
      return null;
    }

    return (await MapAsync(new[] { realm }, cancellationToken)).Single();
  }
  private async Task<IEnumerable<Realm>> MapAsync(IEnumerable<RealmEntity> realms, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = ActorHelper.GetIds(realms.ToArray());
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return realms.Select(mapper.Map);
  }
}
