using AutoMapper;
using Logitar.Portal.v2.Contracts.Users;
using Logitar.Portal.v2.Core.Users;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Queriers;

internal class UserQuerier : IUserQuerier
{
  private readonly IMapper _mapper;
  private readonly DbSet<UserEntity> _users;

  public UserQuerier(PortalContext context, IMapper mapper)
  {
    _mapper = mapper;
    _users = context.Users;
  }

  public async Task<User> GetAsync(UserAggregate user, CancellationToken cancellationToken)
  {
    UserEntity? entity = await _users.AsNoTracking()
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == user.Id.Value, cancellationToken);

    return _mapper.Map<User>(entity);
  }
}
