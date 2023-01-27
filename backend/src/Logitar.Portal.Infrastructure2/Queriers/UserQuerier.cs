using Logitar.Portal.Core2.Realms.Models;
using Logitar.Portal.Core2.Users;
using Logitar.Portal.Core2.Users.Models;
using Logitar.Portal.Infrastructure2.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure2.Queriers
{
  internal class UserQuerier : IUserQuerier
  {
    private readonly IMappingService _mapper;
    private readonly DbSet<UserEntity> _users;

    public UserQuerier(PortalContext context, IMappingService mapper)
    {
      _mapper = mapper;
      _users = context.Users;
    }

    public async Task<UserModel?> GetAsync(string username, RealmModel? realm, CancellationToken cancellationToken)
    {
      UserEntity? user = await _users.AsNoTracking()
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => x.UsernameNormalized == username.ToUpper()
          && (realm == null ? x.RealmId == null : x.Realm!.AggregateId == realm.Id), cancellationToken);

      return await _mapper.MapAsync<UserModel>(user, cancellationToken);
    }
  }
}
