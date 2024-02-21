using Logitar.EventSourcing;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.DeleteCommands;

internal record DeleteRealmMessagesCommand(RealmAggregate Realm, ActorId ActorId) : INotification;
