using Logitar.Portal.Application.Logging;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record UpdateUserCommand(Guid Id, UpdateUserPayload Payload) : ApplicationRequest, IRequest<User?>
{
  public override IActivity GetActivity()
  {
    if (Payload.Password == null)
    {
      return base.GetActivity();
    }

    UpdateUserCommand command = this.DeepClone();
    if (command.Payload.Password != null)
    {
      if (command.Payload.Password.Current != null)
      {
        command.Payload.Password.Current = command.Payload.Password.Current.Mask();
      }
      command.Payload.Password.New = command.Payload.Password.New.Mask();
    }
    return command;
  }
}
