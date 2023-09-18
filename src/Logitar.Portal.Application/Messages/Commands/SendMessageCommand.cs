using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Messages.Commands;

internal record SendMessageCommand(SendMessagePayload Payload, bool IsDemo = false,
  RealmAggregate? Realm = null, TemplateAggregate? Template = null, UserAggregate? User = null)
    : IRequest<SentMessages>;
