using Logitar.Portal.Application.Accounts.Commands;
using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Application.Accounts
{
  internal class GoogleService : IGoogleService
  {
    private readonly IRequestPipeline _requestPipeline;

    public GoogleService(IRequestPipeline requestPipeline)
    {
      _requestPipeline = requestPipeline;
    }

    public async Task<SessionModel> AuthenticateAsync(string realm, AuthenticateWithGooglePayload payload, string? ipAddress, string? additionalInformation, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new AuthenticateWithGoogleCommand(realm, payload, ipAddress, additionalInformation), cancellationToken);
    }
  }
}
