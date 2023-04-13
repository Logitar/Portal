using Logitar.Portal.Core.Realms;
using MediatR;

namespace Logitar.Portal.Core.Templates.Commands;

internal record DeleteTemplates(RealmAggregate Realm) : IRequest;
