using Logitar.Portal.Core.Accounts.Payloads;
using MediatR;

namespace Logitar.Portal.Core.Accounts.Commands
{
  internal class ResetPasswordCommand : IRequest
  {
    public ResetPasswordCommand(ResetPasswordPayload payload, string realm)
    {
      Payload = payload;
      Realm = realm;
    }

    public ResetPasswordPayload Payload { get; }
    public string Realm { get; }
  }
}
