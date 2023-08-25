using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

internal record InitializeConfigurationCommand(InitializeConfigurationPayload Payload) : IRequest<Session>;
