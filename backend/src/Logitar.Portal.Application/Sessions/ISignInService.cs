using Logitar.Portal.Contracts.Sessions.Models;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Sessions
{
  internal interface ISignInService
  {
    Task<SessionModel> SignInAsync(User user, Realm? realm = null, bool remember = false, string? ipAddress = null, string? additionalInformation = null, CancellationToken cancellationToken = default);
  }
}
