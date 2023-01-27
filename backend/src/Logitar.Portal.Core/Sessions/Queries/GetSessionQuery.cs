using Logitar.Portal.Core.Sessions.Models;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Queries
{
  internal class GetSessionQuery : IRequest<SessionModel?>
  {
    public GetSessionQuery(string id)
    {
      Id = id;
    }

    public string Id { get; }
  }
}
