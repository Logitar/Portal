using Logitar.EventSourcing;
using Logitar.Portal.Domain.Senders.Mailgun;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderMailgunSettingsChangedEvent : DomainEvent, INotification
{
  public ReadOnlyMailgunSettings Settings { get; }

  public SenderMailgunSettingsChangedEvent(ActorId actorId, ReadOnlyMailgunSettings settings)
  {
    ActorId = actorId;
    Settings = settings;
  }
}
