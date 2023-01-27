using Logitar.Portal.Core.Accounts.Payloads;
using MediatR;

namespace Logitar.Portal.Core.Accounts.Commands
{
  internal class RecoverPasswordCommand : IRequest
  {
    public RecoverPasswordCommand(RecoverPasswordPayload payload, string realm)
    {
      Payload = payload;
      Realm = realm;
    }

    public RecoverPasswordPayload Payload { get; }
    public string Realm { get; }
  }
}
