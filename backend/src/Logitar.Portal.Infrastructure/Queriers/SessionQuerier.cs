using Logitar.Portal.Core;
using Logitar.Portal.Core.Sessions;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class SessionQuerier : ISessionQuerier
  {
    private readonly DbSet<Session> _sessions;

    public SessionQuerier(PortalDbContext dbContext)
    {
      _sessions = dbContext.Sessions;
    }

    public async Task<Session?> GetAsync(Guid id, bool readOnly, CancellationToken cancellationToken)
    {
      return await _sessions.ApplyTracking(readOnly)
        .Include(x => x.User).ThenInclude(x => x!.Realm)
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PagedList<Session>> GetPagedAsync(bool? isActive, bool? isPersistent, string? realm, Guid? userId,
      SessionSort? sort, bool desc,
      int? index, int? count,
      bool readOnly, CancellationToken cancellationToken)
    {
      IQueryable<Session> query = _sessions.ApplyTracking(readOnly)
        .Include(x => x.User).ThenInclude(x => x!.Realm);


      if (realm == null)
      {
        query = query.Where(x => x.User != null && x.User.RealmSid == null);
      }
      else
      {
        query = Guid.TryParse(realm, out Guid realmId)
          ? query.Where(x => x.User != null && x.User.Realm != null && x.User.Realm.Id == realmId)
          : query.Where(x => x.User != null && x.User.Realm != null && x.User.Realm.AliasNormalized == realm.ToUpper());
      }

      if (isActive.HasValue)
      {
        query = query.Where(x => x.IsActive == isActive.Value);
      }
      if (isPersistent.HasValue)
      {
        query = query.Where(x => x.IsPersistent == isPersistent.Value);
      }
      if (userId.HasValue)
      {
        query = query.Where(x => x.User != null && x.User.Id == userId.Value);
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        query = sort.Value switch
        {
          SessionSort.IpAddress => desc ? query.OrderByDescending(x => x.IpAddress) : query.OrderBy(x => x.IpAddress),
          SessionSort.SignedOutAt => desc ? query.OrderByDescending(x => x.SignedOutAt) : query.OrderBy(x => x.SignedOutAt),
          SessionSort.UpdatedAt => desc ? query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt) : query.OrderBy(x => x.UpdatedAt ?? x.CreatedAt),
          _ => throw new ArgumentException($"The session sort '{sort}' is not valid.", nameof(sort)),
        };
      }

      query = query.ApplyPaging(index, count);

      Session[] sessions = await query.ToArrayAsync(cancellationToken);

      return new PagedList<Session>(sessions, total);
    }
  }
}
