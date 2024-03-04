using Logitar.Portal.Application.Logging;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record ResetUserPasswordCommand(Guid Id, ResetUserPasswordPayload Payload) : ApplicationRequest, IRequest<User?>
{
  public override IActivity GetActivity()
  {
    ResetUserPasswordCommand command = this.DeepClone();
    command.Payload.Password = Payload.Password.Mask();
    return command;
  }
}
