using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal record DeleteRoleCommand(Guid Id) : Activity, IRequest<Role?>;
