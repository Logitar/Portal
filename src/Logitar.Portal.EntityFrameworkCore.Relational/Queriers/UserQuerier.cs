using Logitar.EventSourcing;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class UserQuerier : IUserQuerier
{
  private readonly IActorService _actorService;
  private readonly IRealmQuerier _realmQuerier;
  private readonly DbSet<UserEntity> _users;

  public UserQuerier(IActorService actorService, PortalContext context, IRealmQuerier realmQuerier)
  {
    _actorService = actorService;
    _realmQuerier = realmQuerier;
    _users = context.Users;
  }

  public async Task<User> ReadAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    return await ReadAsync(user.Id.Value, cancellationToken)
      ?? throw new EntityNotFoundException<UserEntity>(user.Id);
  }
  public async Task<User?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new AggregateId(id).Value, cancellationToken);
  }
  private async Task<User?> ReadAsync(string aggregateId, CancellationToken cancellationToken)
  {
    UserEntity? user = await _users.AsNoTracking()
    .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);
    if (user == null)
    {
      return null;
    }

    Realm? realm = null;
    if (user.TenantId != null)
    {
      AggregateId realmId = new(user.TenantId);
      realm = await _realmQuerier.ReadAsync(realmId, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(realmId);
    }

    return (await MapAsync(realm, cancellationToken, user)).Single();
  }

  public async Task<User?> ReadAsync(string? realmIdOrUniqueSlug, string uniqueName, CancellationToken cancellationToken)
  {
    Realm? realm = null;
    if (!string.IsNullOrWhiteSpace(realmIdOrUniqueSlug))
    {
      realm = await _realmQuerier.FindAsync(realmIdOrUniqueSlug, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(realmIdOrUniqueSlug);
    }

    string? tenantId = realm == null ? null : new AggregateId(realm.Id).Value;
    string uniqueNameNormalized = uniqueName.Trim().ToUpper();

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    if (user == null)
    {
      return null;
    }

    return (await MapAsync(realm, cancellationToken, user)).Single();
  }

  private async Task<IEnumerable<User>> MapAsync(Realm? realm = null, CancellationToken cancellationToken = default, params UserEntity[] users)
  {
    IEnumerable<ActorId> actorIds = users.SelectMany(user => user.GetActorIds()).Distinct();
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return users.Select(user => mapper.ToUser(user, realm));
  }
}
