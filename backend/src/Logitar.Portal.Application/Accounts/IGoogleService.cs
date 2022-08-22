using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Sessions.Models;

namespace Logitar.Portal.Application.Accounts
{
  public interface IGoogleService
  {
    Task<SessionModel> AuthenticateAsync(string realm, AuthenticateWithGooglePayload payload, string? ipAddress = null, string? additionalInformation = null, CancellationToken cancellationToken = default);
  }
}
