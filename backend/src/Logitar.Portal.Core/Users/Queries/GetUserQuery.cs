using Logitar.Portal.Core.Users.Models;
using MediatR;

namespace Logitar.Portal.Core.Users.Queries
{
  internal class GetUserQuery : IRequest<UserModel?>
  {
    public GetUserQuery(string id)
    {
      Id = id;
    }

    public string Id { get; }
  }
}
