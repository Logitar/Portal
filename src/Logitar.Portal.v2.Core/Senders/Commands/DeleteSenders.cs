using Logitar.Portal.v2.Core.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Senders.Commands;

internal record DeleteSenders(RealmAggregate Realm) : IRequest;
