using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal record UpdateRoleCommand(string Id, UpdateRolePayload Payload) : IRequest<Role?>;
