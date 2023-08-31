using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record DeleteUsersCommand(RealmAggregate Realm) : INotification;
