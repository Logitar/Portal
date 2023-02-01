using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Users.Models;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Realms;
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

    public async Task<UserModel?> GetAsync(AggregateId id, CancellationToken cancellationToken)
      => await GetAsync(id.Value, cancellationToken);

    public async Task<UserModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      UserEntity? user = await _users.AsNoTracking()
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

      return await _mapper.MapAsync<UserModel>(user, cancellationToken);
    }

    public async Task<IEnumerable<UserModel>> GetByEmailAsync(string email, Realm realm, CancellationToken cancellationToken)
    {
      UserEntity[] users = await _users.AsNoTracking()
        .Include(x => x.Realm)
        .Where(x => x.Realm!.AggregateId == realm.Id.Value && x.EmailNormalized == email.ToUpper())
        .ToArrayAsync(cancellationToken);

      return await _mapper.MapAsync<UserModel>(users, cancellationToken);
    }
  }
}
