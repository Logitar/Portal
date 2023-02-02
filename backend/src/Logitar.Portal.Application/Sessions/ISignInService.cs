using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Sessions
{
  internal interface ISignInService
  {
    Task<SessionModel> RenewAsync(Session session, string? ipAddress = null, string? additionalInformation = null, CancellationToken cancellationToken = default);
    Task<SessionModel> SignInAsync(User user, Realm? realm = null, bool remember = false, string? ipAddress = null, string? additionalInformation = null, CancellationToken cancellationToken = default);
  }
}
