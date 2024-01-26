using Logitar.EventSourcing;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class RoleQuerier : IRoleQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<RoleEntity> _roles;

  public RoleQuerier(IActorService actorService, IApplicationContext applicationContext, IdentityContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _roles = context.Roles;
  }

  public async Task<Role> ReadAsync(RoleAggregate role, CancellationToken cancellationToken)
  {
    return await ReadAsync(role.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The role entity 'AggregateId={role.Id.Value}' could not be found.");
  }
  public async Task<Role?> ReadAsync(RoleId id, CancellationToken cancellationToken)
    => await ReadAsync(id.Value, cancellationToken);
  public async Task<Role?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    string aggregateId = id.Trim();

    RoleEntity? role = await _roles.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    Realm? realm = _applicationContext.Realm;
    if (role == null || role.TenantId != realm?.Id)
    {
      return null;
    }

    return await MapAsync(role, realm, cancellationToken);
  }

  public async Task<Role?> ReadByUniqueNameAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = uniqueName.Trim().ToUpper();

    Realm? realm = _applicationContext.Realm;
    string? tenantId = realm?.Id;

    RoleEntity? role = await _roles.AsNoTracking()
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    if (role == null)
    {
      return null;
    }

    return await MapAsync(role, realm, cancellationToken);
  }

  private async Task<Role> MapAsync(RoleEntity role, Realm? realm, CancellationToken cancellationToken)
  => (await MapAsync([role], realm, cancellationToken)).Single();
  private async Task<IEnumerable<Role>> MapAsync(IEnumerable<RoleEntity> roles, Realm? realm, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = roles.SelectMany(role => role.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return roles.Select(role => mapper.ToRole(role, realm));
  }
}
