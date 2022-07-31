using Microsoft.EntityFrameworkCore;
using Portal.Core.Sessions;
using Portal.Core.Users;

namespace Portal.Infrastructure.Queriers
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

    public async Task<IEnumerable<Session>> GetActiveAsync(User user, bool readOnly, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(user);

      return await _sessions.ApplyTracking(readOnly)
        .Include(x => x.User).ThenInclude(x => x!.Realm)
        .Where(x => x.UserSid == user.Sid && x.IsActive)
        .ToArrayAsync(cancellationToken);
    }
  }
}
