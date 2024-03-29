using Logitar.EventSourcing;
using Logitar.Portal.Domain.Senders.Twilio;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderTwilioSettingsChangedEvent(ReadOnlyTwilioSettings Settings) : DomainEvent, INotification;
