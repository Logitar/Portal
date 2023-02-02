using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Repositories
{
  internal class SessionRepository : Repository, ISessionRepository
  {
    private readonly DbSet<SessionEntity> _sessions;

    public SessionRepository(PortalContext context, IPublisher publisher) : base(context, publisher)
    {
      _sessions = context.Sessions;
    }

    public async Task<IEnumerable<Session>> LoadActiveByUserAsync(User user, CancellationToken cancellationToken)
    {
      SessionEntity[] sessions = await _sessions.AsNoTracking()
        .Include(x => x.User)
        .Where(x => x.User!.AggregateId == user.Id.Value && x.IsActive)
        .ToArrayAsync(cancellationToken);

      if (!sessions.Any())
      {
        return Enumerable.Empty<Session>();
      }

      return await LoadAsync<Session>(sessions.Select(x => x.AggregateId), cancellationToken);
    }

    public async Task<IEnumerable<Session>> LoadByRealmAsync(Realm realm, CancellationToken cancellationToken)
    {
      SessionEntity[] sessions = await _sessions.AsNoTracking()
        .Include(x => x.User).ThenInclude(x => x!.Realm)
        .Where(x => x.User!.Realm!.AggregateId == realm.Id.Value)
        .ToArrayAsync(cancellationToken);

      return sessions.Any() ? await LoadAsync<Session>(sessions.Select(x => x.AggregateId), cancellationToken) : Enumerable.Empty<Session>();
    }
  }
}
