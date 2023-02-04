using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Application.Accounts
{
  public interface IGoogleService
  {
    Task<SessionModel> AuthenticateAsync(string realm, AuthenticateWithGooglePayload payload, string? ipAddress = null, string? additionalInformation = null, CancellationToken cancellationToken = default);
  }
}
