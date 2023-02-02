using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Sessions
{
  public interface ISessionRepository : IRepository
  {
    Task<IEnumerable<Session>> LoadActiveByUserAsync(User user, CancellationToken cancellationToken = default);
    Task<IEnumerable<Session>> LoadByRealmAsync(Realm realm, CancellationToken cancellationToken = default);
  }
}
