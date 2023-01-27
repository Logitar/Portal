using Logitar.Portal.Core.Realms.Models;
using MediatR;

namespace Logitar.Portal.Core.Realms.Commands
{
  internal class DeleteRealmCommand : IRequest<RealmModel>
  {
    public DeleteRealmCommand(string id)
    {
      Id = id;
    }

    public string Id { get; }
  }
}
