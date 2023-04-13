using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Core.Realms;
using MediatR;

namespace Logitar.Portal.Core.Messages.Commands;

internal record ResolveRecipients(RealmAggregate Realm, IEnumerable<RecipientInput> Recipients,
  string ParamName) : IRequest<Recipients>;
