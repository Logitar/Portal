using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record ReplaceUserCommand(Guid Id, ReplaceUserPayload Payload, long? Version) : Activity, IRequest<User?>
{
  public override IActivity Anonymize()
  {
    if (Payload.Password == null)
    {
      return base.Anonymize();
    }

    ReplaceUserCommand command = this.DeepClone();
    command.Payload.Password = Payload.Password.Mask();
    return command;
  }
}
