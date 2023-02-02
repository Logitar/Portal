using Logitar.Portal.Application.Users;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Repositories
{
  internal class UserRepository : Repository, IUserRepository
  {
    private readonly DbSet<UserEntity> _users;

    public UserRepository(PortalContext context, IPublisher publisher) : base(context, publisher)
    {
      _users = context.Users;
    }

    public async Task<IEnumerable<User>> LoadByEmailAsync(string email, Realm? realm, CancellationToken cancellationToken)
    {
      UserEntity[] users = await _users.AsNoTracking()
        .Include(x => x.Realm)
        .Where(x => (realm == null ? x.RealmId == null : x.Realm!.AggregateId == realm.Id.Value)
          && x.EmailNormalized == email.ToUpper())
        .ToArrayAsync(cancellationToken);

      return users.Any() ? await LoadAsync<User>(users.Select(x => x.AggregateId), cancellationToken) : Enumerable.Empty<User>();
    }

    public async Task<User?> LoadByExternalProviderAsync(Realm realm, string key, string value, CancellationToken cancellationToken)
    {
      UserEntity? user = await _users.AsNoTracking()
        .Include(x => x.ExternalProviders)
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => x.Realm!.AggregateId == realm.Id.Value
          && x.ExternalProviders.Any(y => y.Key == key && y.Value == value), cancellationToken);

      return user == null ? null : await LoadAsync<User>(user.AggregateId, cancellationToken);
    }

    public async Task<User?> LoadByUsernameAsync(string username, Realm? realm, CancellationToken cancellationToken)
    {
      UserEntity? user = await _users.AsNoTracking()
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => (realm == null ? x.RealmId == null : x.Realm!.AggregateId == realm.Id.Value)
          && x.UsernameNormalized == username.ToUpper(), cancellationToken);

      return user == null ? null : await LoadAsync<User>(user.AggregateId, cancellationToken);
    }
  }
}
