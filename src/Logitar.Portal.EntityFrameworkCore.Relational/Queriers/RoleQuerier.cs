using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Domain.Roles;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class RoleQuerier : IRoleQuerier
{
  private readonly IActorService _actorService;
  private readonly IRealmQuerier _realmQuerier;
  private readonly DbSet<RoleEntity> _roles;
  private readonly ISqlHelper _sqlHelper;

  public RoleQuerier(IActorService actorService, PortalContext context, IRealmQuerier realmQuerier, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _realmQuerier = realmQuerier;
    _roles = context.Roles;
    _sqlHelper = sqlHelper;
  }

  public async Task<Role> ReadAsync(RoleAggregate role, CancellationToken cancellationToken)
  {
    return await ReadAsync(role.Id, cancellationToken)
      ?? throw new EntityNotFoundException<RoleEntity>(role.Id);
  }
  public async Task<Role?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new AggregateId(id), cancellationToken);
  }
  private async Task<Role?> ReadAsync(AggregateId id, CancellationToken cancellationToken)
  {
    string aggregateId = id.Value;

    RoleEntity? role = await _roles.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);
    if (role == null)
    {
      return null;
    }

    Realm? realm = null;
    if (role.TenantId != null)
    {
      AggregateId realmId = new(role.TenantId);
      realm = await _realmQuerier.ReadAsync(realmId, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(realmId);
    }

    return (await MapAsync(realm, cancellationToken, role)).Single();
  }

  public async Task<Role?> ReadAsync(string? realmIdOrUniqueSlug, string uniqueName, CancellationToken cancellationToken)
  {
    Realm? realm = null;
    if (!string.IsNullOrWhiteSpace(realmIdOrUniqueSlug))
    {
      realm = await _realmQuerier.FindAsync(realmIdOrUniqueSlug, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(realmIdOrUniqueSlug);
    }

    string? tenantId = realm == null ? null : new AggregateId(realm.Id).Value;
    string uniqueNameNormalized = uniqueName.Trim().ToUpper();

    RoleEntity? role = await _roles.AsNoTracking()
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    if (role == null)
    {
      return null;
    }

    return (await MapAsync(realm, cancellationToken, role)).Single();
  }

  public async Task<SearchResults<Role>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken)
  {
    Realm? realm = null;
    string? tenantId = null;
    if (!string.IsNullOrWhiteSpace(payload.Realm))
    {
      realm = await _realmQuerier.FindAsync(payload.Realm, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(payload.Realm);
      tenantId = new AggregateId(realm.Id).Value;
    }

    IQueryBuilder builder = _sqlHelper.QueryFrom(Db.Roles.Table)
      .ApplyIdInFilter(Db.Roles.AggregateId, payload.IdIn)
      .Where(Db.Roles.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .SelectAll(Db.Roles.Table);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Roles.UniqueName, Db.Roles.DisplayName);

    IQueryable<RoleEntity> query = _roles.FromQuery(builder.Build())
      .AsNoTracking();
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<RoleEntity>? ordered = null;
    foreach (RoleSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case RoleSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case RoleSort.UniqueName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case RoleSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    RoleEntity[] roles = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Role> results = await MapAsync(realm, cancellationToken, roles);

    return new SearchResults<Role>(results, total);
  }

  private async Task<IEnumerable<Role>> MapAsync(Realm? realm = null, CancellationToken cancellationToken = default, params RoleEntity[] roles)
  {
    IEnumerable<ActorId> actorIds = roles.SelectMany(role => role.GetActorIds()).Distinct();
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return roles.Select(role => mapper.ToRole(role, realm));
  }
}
