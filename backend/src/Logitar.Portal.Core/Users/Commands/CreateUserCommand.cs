using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Core.Users.Payloads;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands
{
  internal class CreateUserCommand : IRequest<UserModel>
  {
    public CreateUserCommand(CreateUserPayload payload)
    {
      Payload = payload;
    }

    public CreateUserPayload Payload { get; }
  }
}
