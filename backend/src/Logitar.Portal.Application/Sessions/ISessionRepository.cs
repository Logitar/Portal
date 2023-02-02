using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Sessions
{
  public interface ISessionRepository
  {
    Task<IEnumerable<Session>> LoadActiveByUserAsync(User user, CancellationToken cancellationToken = default);
  }
}
