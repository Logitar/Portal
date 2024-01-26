using Logitar.EventSourcing;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class UserQuerier : IUserQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<UserEntity> _users;

  public UserQuerier(IActorService actorService, IApplicationContext applicationContext, IdentityContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _users = context.Users;
  }

  public async Task<User> ReadAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    return await ReadAsync(user.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity 'AggregateId={user.Id.Value}' could not be found.");
  }
  public async Task<User?> ReadAsync(UserId id, CancellationToken cancellationToken)
    => await ReadAsync(id.Value, cancellationToken);
  public async Task<User?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    string aggregateId = id.Trim();

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    Realm? realm = _applicationContext.Realm;
    if (user == null || user.TenantId != realm?.Id)
    {
      return null;
    }

    return await MapAsync(user, realm, cancellationToken);
  }

  private async Task<User> MapAsync(UserEntity user, Realm? realm, CancellationToken cancellationToken)
  => (await MapAsync([user], realm, cancellationToken)).Single();
  private async Task<IEnumerable<User>> MapAsync(IEnumerable<UserEntity> users, Realm? realm, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = users.SelectMany(user => user.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return users.Select(user => mapper.ToUser(user, realm));
  }
}
