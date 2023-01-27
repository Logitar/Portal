using Logitar.Portal.Core.Configurations.Payloads;
using MediatR;

namespace Logitar.Portal.Core.Configurations.Commands
{
  internal class InitializeConfigurationCommand : IRequest
  {
    public InitializeConfigurationCommand(InitializeConfigurationPayload payload)
    {
      Payload = payload;
    }

    public InitializeConfigurationPayload Payload { get; }
  }
}
