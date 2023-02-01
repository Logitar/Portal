using Logitar.Portal.Application.Users;
using Logitar.Portal.Domain;
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

    public async Task<User?> LoadByUsernameAsync(string username, AggregateId? realmId, CancellationToken cancellationToken)
    {
      UserEntity? user = await _users.AsNoTracking()
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => (realmId.HasValue ? x.Realm!.AggregateId == realmId.Value.Value : x.RealmId == null)
          && x.UsernameNormalized == username.ToUpper(), cancellationToken);

      return user == null ? null : await LoadAsync<User>(user.AggregateId, cancellationToken);
    }
  }
}
