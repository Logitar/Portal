using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Templates;
using MediatR;

namespace Logitar.Portal.Core.Messages.Commands;

internal record ResolveTemplate(RealmAggregate Realm, string IdOrUniqueName,
  string ParamName) : IRequest<TemplateAggregate>;
