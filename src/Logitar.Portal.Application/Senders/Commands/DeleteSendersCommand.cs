using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal record DeleteSendersCommand(RealmAggregate Realm) : INotification;
