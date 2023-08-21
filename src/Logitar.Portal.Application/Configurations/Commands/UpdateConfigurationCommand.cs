using Logitar.Portal.Contracts.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

internal record UpdateConfigurationCommand(UpdateConfigurationPayload Payload) : IRequest<Configuration?>;
