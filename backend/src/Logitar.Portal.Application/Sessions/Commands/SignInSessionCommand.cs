using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

public record SignInSessionCommand(SignInSessionPayload Payload) : Activity, IRequest<SessionModel>
{
  public override IActivity Anonymize()
  {
    SignInSessionCommand command = this.DeepClone();
    command.Payload.Password = Payload.Password.Mask();
    return command;
  }
}
