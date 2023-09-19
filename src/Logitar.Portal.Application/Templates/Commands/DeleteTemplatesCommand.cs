using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal record DeleteTemplatesCommand(RealmAggregate Realm) : INotification;
