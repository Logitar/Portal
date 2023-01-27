using Logitar.Portal.Core.Users.Models;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands
{
  internal class EnableUserCommand : IRequest<UserModel>
  {
    public EnableUserCommand(string id)
    {
      Id = id;
    }

    public string Id { get; }
  }
}
