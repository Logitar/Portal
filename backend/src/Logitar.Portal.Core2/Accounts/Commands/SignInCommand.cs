using Logitar.Portal.Core2.Accounts.Payloads;
using Logitar.Portal.Core2.Sessions.Models;
using MediatR;

namespace Logitar.Portal.Core2.Accounts.Commands
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
