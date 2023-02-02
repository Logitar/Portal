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
      throw new NotImplementedException(); // TODO(fpion): implement
    }
  }
}
