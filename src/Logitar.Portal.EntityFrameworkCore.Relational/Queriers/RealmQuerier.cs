using Logitar.Data;
using Logitar.EventSourcing;
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
  private readonly DbSet<RealmEntity> _realms;
  private readonly ISqlHelper _sqlHelper;

  public RealmQuerier(IActorService actorService, PortalContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _realms = context.Realms;
    _sqlHelper = sqlHelper;
  }

  public async Task<Realm?> FindAsync(string idOrUniqueSlug, CancellationToken cancellationToken)
  {
    idOrUniqueSlug = idOrUniqueSlug.Trim();
    string uniqueSlugNormalized = idOrUniqueSlug.ToUpper();

    string? aggregateId = null;
    if (Guid.TryParse(idOrUniqueSlug, out Guid id))
    {
      aggregateId = new AggregateId(id).Value;
    }

    RealmEntity[] realms = await _realms.AsNoTracking()
      .Include(x => x.PasswordRecoverySender)
      .Include(x => x.PasswordRecoveryTemplate)
      .Where(x => x.AggregateId == aggregateId || x.UniqueSlugNormalized == uniqueSlugNormalized)
      .ToArrayAsync(cancellationToken);
    if (realms.Length == 0)
    {
      return null;
    }
    RealmEntity realm = realms.Length == 1 ? realms.Single() : realms.First(realm => realm.AggregateId == aggregateId);

    return (await MapAsync(cancellationToken, realm)).Single();
  }

  public async Task<Realm> ReadAsync(RealmAggregate realm, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm.Id, cancellationToken)
      ?? throw new EntityNotFoundException<RealmEntity>(realm.Id);
  }
  public async Task<Realm?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new AggregateId(id), cancellationToken);
  }
  public async Task<Realm?> ReadAsync(AggregateId id, CancellationToken cancellationToken)
  {
    string aggregateId = id.Value;

    RealmEntity? realm = await _realms.AsNoTracking()
      .Include(x => x.PasswordRecoverySender)
      .Include(x => x.PasswordRecoveryTemplate)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);
    if (realm == null)
    {
      return null;
    }

    return (await MapAsync(cancellationToken, realm)).Single();
  }

  public async Task<Realm?> ReadAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = uniqueSlug.Trim().ToUpper();

    RealmEntity? realm = await _realms.AsNoTracking()
      .Include(x => x.PasswordRecoverySender)
      .Include(x => x.PasswordRecoveryTemplate)
      .SingleOrDefaultAsync(x => x.UniqueSlugNormalized == uniqueSlugNormalized, cancellationToken);
    if (realm == null)
    {
      return null;
    }

    return (await MapAsync(cancellationToken, realm)).Single();
  }

  public async Task<SearchResults<Realm>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(Db.Realms.Table)
      .ApplyIdInFilter(Db.Realms.AggregateId, payload.IdIn)
      .SelectAll(Db.Realms.Table);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Realms.UniqueSlug, Db.Realms.DisplayName);

    IQueryable<RealmEntity> query = _realms.FromQuery(builder.Build()).AsNoTracking()
      .Include(x => x.PasswordRecoverySender)
      .Include(x => x.PasswordRecoveryTemplate);
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
    IEnumerable<Realm> results = await MapAsync(cancellationToken, realms);

    return new SearchResults<Realm>(results, total);
  }

  private async Task<IEnumerable<Realm>> MapAsync(CancellationToken cancellationToken = default, params RealmEntity[] realms)
  {
    IEnumerable<ActorId> actorIds = realms.SelectMany(realm => realm.GetActorIds()).Distinct();
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return realms.Select(mapper.ToRealm);
  }
}
