using Logitar.EventSourcing;
using Logitar.Portal.Domain.Senders.SendGrid;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderSendGridSettingsChangedEvent : DomainEvent, INotification
{
  public ReadOnlySendGridSettings Settings { get; }

  public SenderSendGridSettingsChangedEvent(ActorId actorId, ReadOnlySendGridSettings settings)
  {
    ActorId = actorId;
    Settings = settings;
  }
}
