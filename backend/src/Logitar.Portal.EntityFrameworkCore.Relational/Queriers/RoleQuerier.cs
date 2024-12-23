using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class RoleQuerier : IRoleQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<RoleEntity> _roles;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public RoleQuerier(IActorService actorService, IdentityContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _roles = context.Roles;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<RoleModel> ReadAsync(Realm? realm, Role role, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, role.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The role entity 'AggregateId={role.Id.Value}' could not be found.");
  }
  public async Task<RoleModel?> ReadAsync(Realm? realm, RoleId id, CancellationToken cancellationToken)
    => await ReadAsync(realm, id.ToGuid(), cancellationToken);
  public async Task<RoleModel?> ReadAsync(Realm? realm, Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    RoleEntity? role = await _roles.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    if (role == null || role.TenantId != realm?.GetTenantId().Value)
    {
      return null;
    }

    return await MapAsync(role, realm, cancellationToken);
  }

  public async Task<RoleModel?> ReadAsync(Realm? realm, string uniqueName, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.GetTenantId().Value;
    string uniqueNameNormalized = uniqueName.Trim().ToUpper();

    RoleEntity? role = await _roles.AsNoTracking()
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    if (role == null)
    {
      return null;
    }

    return await MapAsync(role, realm, cancellationToken);
  }

  public async Task<SearchResults<RoleModel>> SearchAsync(Realm? realm, SearchRolesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(IdentityDb.Roles.Table).SelectAll(IdentityDb.Roles.Table)
      .ApplyRealmFilter(IdentityDb.Roles.TenantId, realm)
      .ApplyIdFilter(IdentityDb.Roles.AggregateId, payload.Ids);
    _searchHelper.ApplyTextSearch(builder, payload.Search, IdentityDb.Roles.UniqueName, IdentityDb.Roles.DisplayName);

    IQueryable<RoleEntity> query = _roles.FromQuery(builder).AsNoTracking();

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
    IEnumerable<RoleModel> items = await MapAsync(roles, realm, cancellationToken);

    return new SearchResults<RoleModel>(items, total);
  }

  private async Task<RoleModel> MapAsync(RoleEntity role, Realm? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([role], realm, cancellationToken)).Single();
  private async Task<IEnumerable<RoleModel>> MapAsync(IEnumerable<RoleEntity> roles, Realm? realm, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = roles.SelectMany(role => role.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return roles.Select(role => mapper.ToRole(role, realm));
  }
}
