using Logitar.Portal.Core2.Sessions.Models;
using Logitar.Portal.Core2.Users;

namespace Logitar.Portal.Core2.Sessions
{
  internal interface ISignInService
  {
    Task<SessionModel> SignInAsync(User user, bool remember = false, string? ipAddress = null, string? additionalInformation = null, CancellationToken cancellationToken = default);
  }
}
