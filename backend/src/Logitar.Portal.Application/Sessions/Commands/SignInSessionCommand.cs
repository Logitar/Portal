using Logitar.Portal.Application.Logging;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal record SignInSessionCommand(SignInSessionPayload Payload) : ApplicationRequest, IRequest<Session>
{
  public override IActivity GetActivity()
  {
    SignInSessionCommand command = this.DeepClone();
    command.Payload.Password = Payload.Password.Mask();
    return command;
  }
}
