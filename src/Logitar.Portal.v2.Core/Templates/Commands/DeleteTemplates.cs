using Logitar.Portal.v2.Core.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Templates.Commands;

internal record DeleteTemplates(RealmAggregate Realm) : IRequest;
