using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Sessions.Models;
using MediatR;

namespace Logitar.Portal.Core.Accounts.Commands
{
  internal class SignInCommand : IRequest<SessionModel>
  {
    public SignInCommand(SignInPayload payload, string? realm)
    {
      Payload = payload;
      Realm = realm;
    }

    public SignInPayload Payload { get; }
    public string? Realm { get; }
  }
}
