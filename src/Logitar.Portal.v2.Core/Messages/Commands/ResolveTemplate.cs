using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Templates;
using MediatR;

namespace Logitar.Portal.v2.Core.Messages.Commands;

internal record ResolveTemplate(RealmAggregate Realm, string IdOrUniqueName,
  string ParamName) : IRequest<TemplateAggregate>;
