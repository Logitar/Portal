using Logitar.Portal.Application.Configurations.Payloads;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands
{
  internal record InitializeConfigurationCommand(InitializeConfigurationPayload Payload) : IRequest;
}
