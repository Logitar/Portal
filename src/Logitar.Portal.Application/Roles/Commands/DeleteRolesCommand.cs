using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal record DeleteRolesCommand(RealmAggregate Realm) : INotification;
