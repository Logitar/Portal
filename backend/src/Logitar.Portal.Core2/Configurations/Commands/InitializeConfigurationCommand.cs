using Logitar.Portal.Core2.Configurations.Payloads;
using MediatR;

namespace Logitar.Portal.Core2.Configurations.Commands
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
