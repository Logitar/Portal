using Logitar.Portal.Core.Users.Models;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands
{
  internal class DeleteUserCommand : IRequest<UserModel>
  {
    public DeleteUserCommand(string id)
    {
      Id = id;
    }

    public string Id { get; }
  }
}
