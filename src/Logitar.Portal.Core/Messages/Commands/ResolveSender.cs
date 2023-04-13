using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Senders;
using MediatR;

namespace Logitar.Portal.Core.Messages.Commands;

internal record ResolveSender(RealmAggregate Realm, Guid? Id, string ParamName) : IRequest<SenderAggregate>;
