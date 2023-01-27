using Logitar.Portal.Core.Models;
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

    public async Task<UserModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      UserEntity? user = await _users.AsNoTracking()
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

      return user == null ? null : await _mapper.MapAsync<UserModel>(user, cancellationToken);
    }

    public async Task<UserModel?> GetAsync(string username, string? realm, CancellationToken cancellationToken)
    {
      UserEntity? user = await _users.AsNoTracking()
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => x.UsernameNormalized == username.ToUpper() && (realm == null
          ? x.RealmId == null
          : (x.Realm!.AggregateId == realm || x.Realm.AliasNormalized == realm.ToUpper())), cancellationToken);

      return user == null ? null : await _mapper.MapAsync<UserModel>(user, cancellationToken);
    }

    public async Task<IEnumerable<UserModel>> GetByEmailAsync(string email, string? realm, CancellationToken cancellationToken)
    {
      UserEntity[] users = await _users.AsNoTracking()
        .Include(x => x.Realm)
        .Where(x => x.EmailNormalized == email.ToUpper() && (realm == null
          ? x.RealmId == null
          : (x.Realm!.AggregateId == realm || x.Realm.AliasNormalized == realm.ToUpper())))
        .ToArrayAsync(cancellationToken);

      return await _mapper.MapAsync<UserModel>(users, cancellationToken);
    }

    public async Task<ListModel<UserModel>> GetPagedAsync(bool? isConfirmed, bool? isDisabled, string? realm, string? search,
      UserSort? sort, bool isDescending,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      IQueryable<UserEntity> query = _users.AsNoTracking()
        .Include(x => x.Realm);

      query = realm == null
        ? query.Where(x => x.RealmId == null)
        : query.Where(x => x.Realm!.AggregateId == realm || x.Realm.AliasNormalized == realm.ToUpper());

      if (isConfirmed.HasValue)
      {
        query = query.Where(x => x.IsAccountConfirmed == isConfirmed.Value);
      }
      if (isDisabled.HasValue)
      {
        query = query.Where(x => x.IsDisabled == isDisabled.Value);
      }
      if (search != null)
      {
        foreach (string term in search.Split())
        {
          if (!string.IsNullOrEmpty(term))
          {
            string pattern = $"%{term}%";

            query = query.Where(x => EF.Functions.ILike(x.Username, pattern)
              || (x.Email != null && EF.Functions.ILike(x.Email, pattern))
              || (x.PhoneNumber != null && EF.Functions.ILike(x.PhoneNumber, pattern))
              || (x.FirstName != null && EF.Functions.ILike(x.FirstName, pattern))
              || (x.MiddleName != null && EF.Functions.ILike(x.MiddleName, pattern))
              || (x.LastName != null && EF.Functions.ILike(x.LastName, pattern)));
          }
        }
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        query = sort.Value switch
        {
          UserSort.Email => isDescending ? query.OrderByDescending(x => x.Email) : query.OrderBy(x => x.Email),
          UserSort.FirstMiddleLastName => isDescending
            ? query.OrderByDescending(x => x.FirstName).ThenByDescending(x => x.MiddleName).ThenByDescending(x => x.LastName)
            : query.OrderBy(x => x.FirstName).ThenBy(x => x.MiddleName).ThenBy(x => x.LastName),
          UserSort.LastFirstMiddleName => isDescending
            ? query.OrderByDescending(x => x.LastName).ThenByDescending(x => x.FirstName).ThenByDescending(x => x.MiddleName)
            : query.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ThenBy(x => x.MiddleName),
          UserSort.PhoneNumber => isDescending ? query.OrderByDescending(x => x.PhoneNumber) : query.OrderBy(x => x.PhoneNumber),
          UserSort.PasswordChangedAt => isDescending ? query.OrderByDescending(x => x.PasswordChangedOn) : query.OrderBy(x => x.PasswordChangedOn),
          UserSort.SignedInAt => isDescending ? query.OrderByDescending(x => x.SignedInOn) : query.OrderBy(x => x.SignedInOn),
          UserSort.UpdatedOn => isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn),
          UserSort.Username => isDescending ? query.OrderByDescending(x => x.Username) : query.OrderBy(x => x.Username),
          _ => throw new ArgumentException($"The user sort '{sort}' is not valid.", nameof(sort)),
        };
      }

      query = query.ApplyPaging(index, count);

      UserEntity[] users = await query.ToArrayAsync(cancellationToken);

      return new ListModel<UserModel>(await _mapper.MapAsync<UserModel>(users, cancellationToken), total);
    }
  }
}
