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
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<RoleEntity> _roles;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public RoleQuerier(IActorService actorService, IApplicationContext applicationContext,
    IdentityContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _roles = context.Roles;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<Role> ReadAsync(RoleAggregate role, CancellationToken cancellationToken)
  {
    return await ReadAsync(role.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The role entity 'AggregateId={role.Id.Value}' could not be found.");
  }
  public async Task<Role?> ReadAsync(RoleId id, CancellationToken cancellationToken)
    => await ReadAsync(id.ToGuid(), cancellationToken);
  public async Task<Role?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    RoleEntity? role = await _roles.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    Realm? realm = _applicationContext.Realm;
    if (role == null || role.TenantId != _applicationContext.TenantId?.Value)
    {
      return null;
    }

    return await MapAsync(role, realm, cancellationToken);
  }

  public async Task<Role?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string? tenantId = _applicationContext.TenantId?.Value;
    string uniqueNameNormalized = uniqueName.Trim().ToUpper();

    RoleEntity? role = await _roles.AsNoTracking()
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    if (role == null)
    {
      return null;
    }

    return await MapAsync(role, _applicationContext.Realm, cancellationToken);
  }

  public async Task<SearchResults<Role>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken)
  {
    Realm? realm = _applicationContext.Realm;

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
    IEnumerable<Role> items = await MapAsync(roles, realm, cancellationToken);

    return new SearchResults<Role>(items, total);
  }

  private async Task<Role> MapAsync(RoleEntity role, Realm? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([role], realm, cancellationToken)).Single();
  private async Task<IEnumerable<Role>> MapAsync(IEnumerable<RoleEntity> roles, Realm? realm, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = roles.SelectMany(role => role.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return roles.Select(role => mapper.ToRole(role, realm));
  }
}
