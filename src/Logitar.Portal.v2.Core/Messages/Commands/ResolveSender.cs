using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Senders;
using MediatR;

namespace Logitar.Portal.v2.Core.Messages.Commands;

internal record ResolveSender(RealmAggregate Realm, Guid? Id, string ParamName) : IRequest<SenderAggregate>;
