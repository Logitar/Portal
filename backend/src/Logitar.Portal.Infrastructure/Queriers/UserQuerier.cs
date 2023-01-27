using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Core.Users;
using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Queriers
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
