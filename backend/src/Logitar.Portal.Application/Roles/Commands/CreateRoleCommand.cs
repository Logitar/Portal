using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal record CreateRoleCommand(CreateRolePayload Payload) : Activity, IRequest<RoleModel>;
