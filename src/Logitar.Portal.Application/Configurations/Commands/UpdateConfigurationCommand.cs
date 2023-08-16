using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

public record UpdateConfigurationCommand(UpdateConfigurationPayload Payload) : IRequest<Configuration>;
