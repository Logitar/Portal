using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Messages.Commands;

internal record DeleteMessagesCommand(RealmAggregate Realm) : INotification;
