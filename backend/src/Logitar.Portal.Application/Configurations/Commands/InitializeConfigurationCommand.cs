using Logitar.Portal.Application.Logging;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

internal record InitializeConfigurationCommand(InitializeConfigurationPayload Payload) : ApplicationRequest, IRequest<Session>
{
  public override IActivity GetActivity()
  {
    InitializeConfigurationCommand command = this.DeepClone();
    command.Payload.User.Password = Payload.User.Password.Mask();
    return command;
  }
}
