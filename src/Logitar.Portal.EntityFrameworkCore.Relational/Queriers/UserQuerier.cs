using AutoMapper;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class UserQuerier : IUserQuerier
{
  private readonly IMapper _mapper;
  private readonly DbSet<RealmEntity> _realms;
  private readonly DbSet<UserEntity> _users;

  public UserQuerier(IdentityContext identityContext, IMapper mapper, PortalContext portalContext)
  {
    _mapper = mapper;
    _realms = portalContext.Realms;
    _users = identityContext.Users;
  }

  public async Task<User> ReadAsync(UserAggregate user, CancellationToken cancellationToken)
    => await ReadAsync(user.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity 'AggregateId={user.Id}' could not be found.");
  public async Task<User?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    UserEntity? user = await _users.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

    return _mapper.Map<User?>(user);
  }

  public async Task<User?> ReadAsync(string? realmIdOrUniqueSlug, string uniqueName, CancellationToken cancellationToken)
  {
    RealmEntity? realm = null;
    if (realmIdOrUniqueSlug != null)
    {
      string uniqueSlugNormalized = realmIdOrUniqueSlug.ToUpper();
      realm = await _realms.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == realmIdOrUniqueSlug || x.UniqueSlugNormalized == uniqueSlugNormalized, cancellationToken);
      if (realm == null)
      {
        return null;
      }
    }

    string? tenantId = realm?.AggregateId;
    string uniqueNameNormalized = uniqueName.Trim().ToUpper();

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    User? result = _mapper.Map<User?>(user);
    if (result != null && realm != null)
    {
      result.Realm = _mapper.Map<Realm>(realm);
    }

    return result;
  }
}
