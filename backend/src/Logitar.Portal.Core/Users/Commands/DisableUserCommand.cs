using Logitar.Portal.Core.Users.Models;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands
{
  internal class DisableUserCommand : IRequest<UserModel>
  {
    public DisableUserCommand(string id)
    {
      Id = id;
    }

    public string Id { get; }
  }
}
