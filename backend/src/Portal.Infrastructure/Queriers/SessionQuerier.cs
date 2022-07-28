using Microsoft.EntityFrameworkCore;
using Portal.Core.Sessions;

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
  }
}
