using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Core.Realms.Payloads;
using MediatR;

namespace Logitar.Portal.Core.Realms.Commands
{
  internal class UpdateRealmCommand : IRequest<RealmModel>
  {
    public UpdateRealmCommand(string id, UpdateRealmPayload payload)
    {
      Id = id;
      Payload = payload;
    }

    public string Id { get; }
    public UpdateRealmPayload Payload { get; }
  }
}
