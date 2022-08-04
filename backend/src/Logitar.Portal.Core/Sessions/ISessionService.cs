using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Sessions
{
  public interface ISessionService
  {
    Task<SessionModel> RenewAsync(Session session, string? ipAddress = null, string? additionalInformation = null, CancellationToken cancellationToken = default);
    Task<SessionModel> SignInAsync(User user, bool remember = false, string? ipAddress = null, string? additionalInformation = null, CancellationToken cancellationToken = default);
  }
}
