using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Core.Realms.Payloads;
using MediatR;

namespace Logitar.Portal.Core.Realms.Commands
{
  internal class CreateRealmCommand : IRequest<RealmModel>
  {
    public CreateRealmCommand(CreateRealmPayload payload)
    {
      Payload = payload;
    }

    public CreateRealmPayload Payload { get; }
  }
}
