using Logitar.Portal.v2.Contracts.Messages;
using Logitar.Portal.v2.Core.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Messages.Commands;

internal record ResolveRecipients(RealmAggregate Realm, IEnumerable<RecipientInput> Recipients,
  string ParamName) : IRequest<Recipients>;
