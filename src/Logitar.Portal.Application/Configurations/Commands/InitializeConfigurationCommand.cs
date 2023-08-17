using Logitar.Portal.Contracts.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

public record InitializeConfigurationCommand(InitializeConfigurationPayload Payload) : IRequest<Unit>;
