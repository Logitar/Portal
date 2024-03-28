using Logitar.EventSourcing;
using Logitar.Portal.Domain.Senders.Twilio;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderTwilioSettingsChangedEvent : DomainEvent, INotification
{
  public ReadOnlyTwilioSettings Settings { get; }

  public SenderTwilioSettingsChangedEvent(ActorId actorId, ReadOnlyTwilioSettings settings)
  {
    ActorId = actorId;
    Settings = settings;
  }
}
