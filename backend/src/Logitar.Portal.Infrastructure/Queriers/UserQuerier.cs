using Logitar.Portal.Core;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class UserQuerier : IUserQuerier
  {
    private readonly DbSet<User> _users;

    public UserQuerier(PortalDbContext dbContext)
    {
      _users = dbContext.Users;
    }

    public async Task<User?> GetAsync(string username, Realm? realm, bool readOnly, CancellationToken cancellationToken)
    {
      username = username?.ToUpper() ?? throw new ArgumentNullException(nameof(username));
      int? realmSid = realm?.Sid;

      return await _users.ApplyTracking(readOnly)
        .Include(x => x.ExternalProviders)
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => x.RealmSid == realmSid && x.UsernameNormalized == username, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAsync(IEnumerable<string> usernames, Realm? realm, bool readOnly, CancellationToken cancellationToken)
    {
      usernames = usernames?.Select(x => x.ToUpper()) ?? throw new ArgumentNullException(nameof(usernames));
      int? realmSid = realm?.Sid;

      return await _users.ApplyTracking(readOnly)
        .Include(x => x.Realm)
        .Where(x => x.RealmSid == realmSid && usernames.Contains(x.UsernameNormalized))
        .ToArrayAsync(cancellationToken);
    }

    public async Task<User?> GetAsync(Guid id, bool readOnly, CancellationToken cancellationToken)
    {
      return await _users.ApplyTracking(readOnly)
        .Include(x => x.ExternalProviders)
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAsync(IEnumerable<Guid> ids, bool readOnly, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(ids);

      return await _users.ApplyTracking(readOnly)
        .Include(x => x.Realm)
        .Where(x => ids.Contains(x.Id))
        .ToArrayAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetByEmailAsync(string email, Realm? realm, bool readOnly, CancellationToken cancellationToken)
    {
      email = email?.ToUpper() ?? throw new ArgumentNullException(nameof(email));
      int? realmSid = realm?.Sid;

      return await _users.ApplyTracking(readOnly)
        .Include(x => x.Realm)
        .Where(x => x.RealmSid == realmSid && x.EmailNormalized == email)
        .ToArrayAsync(cancellationToken);
    }

    public async Task<User?> GetByExternalProviderAsync(Realm realm, string providerKey, string providerValue, bool readOnly, CancellationToken cancellationToken = default)
    {
      ArgumentNullException.ThrowIfNull(realm);
      ArgumentNullException.ThrowIfNull(providerKey);
      ArgumentNullException.ThrowIfNull(providerValue);

      return await _users.ApplyTracking(readOnly)
        .Include(x => x.ExternalProviders)
        .Include(x => x.Realm)
        .Where(x => x.ExternalProviders.Any(y => y.RealmSid == realm.Sid && y.Key == providerKey && y.Value == providerValue))
        .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedList<User>> GetPagedAsync(bool? isConfirmed, bool? isDisabled, string? realm, string? search,
      UserSort? sort, bool desc,
      int? index, int? count,
      bool readOnly, CancellationToken cancellationToken)
    {
      IQueryable<User> query = _users.ApplyTracking(readOnly)
        .Include(x => x.Realm);

      if (realm == null)
      {
        query = query.Where(x => x.RealmSid == null);
      }
      else
      {
        query = Guid.TryParse(realm, out Guid realmId)
          ? query.Where(x => x.Realm != null && x.Realm.Id == realmId)
          : query.Where(x => x.Realm != null && x.Realm.AliasNormalized == realm.ToUpper());
      }

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
          UserSort.Email => desc ? query.OrderByDescending(x => x.Email) : query.OrderBy(x => x.Email),
          UserSort.FirstMiddleLastName => desc
            ? query.OrderByDescending(x => x.FirstName).ThenByDescending(x => x.MiddleName).ThenByDescending(x => x.LastName)
            : query.OrderBy(x => x.FirstName).ThenBy(x => x.MiddleName).ThenBy(x => x.LastName),
          UserSort.LastFirstMiddleName => desc
            ? query.OrderByDescending(x => x.LastName).ThenByDescending(x => x.FirstName).ThenByDescending(x => x.MiddleName)
            : query.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ThenBy(x => x.MiddleName),
          UserSort.PhoneNumber => desc ? query.OrderByDescending(x => x.PhoneNumber) : query.OrderBy(x => x.PhoneNumber),
          UserSort.PasswordChangedAt => desc ? query.OrderByDescending(x => x.PasswordChangedAt) : query.OrderBy(x => x.PasswordChangedAt),
          UserSort.SignedInAt => desc ? query.OrderByDescending(x => x.SignedInAt) : query.OrderBy(x => x.SignedInAt),
          UserSort.UpdatedAt => desc ? query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt) : query.OrderBy(x => x.UpdatedAt ?? x.CreatedAt),
          UserSort.Username => desc ? query.OrderByDescending(x => x.Username) : query.OrderBy(x => x.Username),
          _ => throw new ArgumentException($"The user sort '{sort}' is not valid.", nameof(sort)),
        };
      }

      query = query.ApplyPaging(index, count);

      User[] users = await query.ToArrayAsync(cancellationToken);

      return new PagedList<User>(users, total);
    }
  }
}
