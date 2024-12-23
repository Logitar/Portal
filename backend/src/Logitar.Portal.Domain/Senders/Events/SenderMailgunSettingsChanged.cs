using Logitar.EventSourcing;
using Logitar.Portal.Domain.Senders.Mailgun;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderMailgunSettingsChanged(ReadOnlyMailgunSettings Settings) : DomainEvent, INotification;
