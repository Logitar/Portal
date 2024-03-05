using Logitar.Portal.Application.Logging;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record AuthenticateUserCommand(AuthenticateUserPayload Payload) : ApplicationRequest, IRequest<User>
{
  public override IActivity GetActivity()
  {
    AuthenticateUserCommand command = this.DeepClone();
    command.Payload.Password = Payload.Password.Mask();
    return command;
  }
}
