using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal record ReplaceRoleCommand(string Id, ReplaceRolePayload Payload, long? Version) : IRequest<Role?>;
