using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

public record ReplaceConfigurationCommand(ReplaceConfigurationPayload Payload, long? Version)
  : IRequest<Configuration>;
