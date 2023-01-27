using Logitar.Portal.Core.Realms.Models;
using MediatR;

namespace Logitar.Portal.Core.Realms.Queries
{
  internal class GetRealmQuery : IRequest<RealmModel?>
  {
    public GetRealmQuery(string id)
    {
      Id = id;
    }

    public string Id { get; }
  }
}
