using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Sessions.Models;
using MediatR;

namespace Logitar.Portal.Core.Accounts.Commands
{
  internal class RenewSessionCommand : IRequest<SessionModel>
  {
    public RenewSessionCommand(RenewSessionPayload payload, string? realm)
    {
      Payload = payload;
      Realm = realm;
    }

    public RenewSessionPayload Payload { get; }
    public string? Realm { get; }
  }
}
