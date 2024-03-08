using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal record UpdateRoleCommand(Guid Id, UpdateRolePayload Payload) : Activity, IRequest<Role?>;
