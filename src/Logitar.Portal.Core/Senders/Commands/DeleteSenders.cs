using Logitar.Portal.Core.Realms;
using MediatR;

namespace Logitar.Portal.Core.Senders.Commands;

internal record DeleteSenders(RealmAggregate Realm) : IRequest;
