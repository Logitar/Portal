using Logitar.Portal.Domain.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal record RemoveRolesCommand(RoleAggregate Role) : INotification;
