using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Accounts.Commands
{
  internal record AuthenticateWithGoogleCommand(string Realm, AuthenticateWithGooglePayload Payload,
    string? IpAddress, string? AdditionalInformation) : IRequest<SessionModel>;
}
