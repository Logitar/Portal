using Logitar.Portal.Application.Logging;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record CreateUserCommand(CreateUserPayload Payload) : ApplicationRequest, IRequest<User>
{
  public override IActivity Anonymize()
  {
    if (Payload.Password == null)
    {
      return base.Anonymize();
    }

    CreateUserCommand command = this.DeepClone();
    command.Payload.Password = Payload.Password.Mask();
    return command;
  }
}
