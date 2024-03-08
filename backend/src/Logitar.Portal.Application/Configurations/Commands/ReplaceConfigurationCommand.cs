using Logitar.Portal.Contracts.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

internal record ReplaceConfigurationCommand(ReplaceConfigurationPayload Payload, long? Version) : Activity,IRequest<Configuration>;
