using Microsoft.EntityFrameworkCore;
using Portal.Core;
using Portal.Core.Realms;
using Portal.Core.Users;

namespace Portal.Infrastructure.Queriers
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

      IQueryable<User> query = _users.ApplyTracking(readOnly).Include(x => x.Realm);

      return realm == null
        ? await query.SingleOrDefaultAsync(x => x.RealmSid == null && x.UsernameNormalized == username, cancellationToken)
        : await query.SingleOrDefaultAsync(x => x.RealmSid == realm.Sid && x.UsernameNormalized == username, cancellationToken);
    }

    public async Task<User?> GetAsync(Guid id, bool readOnly, CancellationToken cancellationToken)
    {
      return await _users.ApplyTracking(readOnly)
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAsync(IEnumerable<Guid> ids, bool readOnly = false, CancellationToken cancellationToken = default)
    {
      ArgumentNullException.ThrowIfNull(ids);

      return await _users.ApplyTracking(readOnly)
        .Include(x => x.Realm)
        .Where(x => ids.Contains(x.Id))
        .ToArrayAsync(cancellationToken);
    }

    public async Task<PagedList<User>> GetPagedAsync(Guid? realmId, string? search,
      UserSort? sort, bool desc,
      int? index, int? count,
      bool readOnly, CancellationToken cancellationToken)
    {
      IQueryable<User> query = _users.ApplyTracking(readOnly)
        .Include(x => x.Realm)
        .Where(x => realmId.HasValue ? (x.Realm != null && x.Realm.Id == realmId.Value) : x.Realm == null);

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
