using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Templates.Events;

public record TemplateDeletedEvent : DomainEvent, INotification
{
  public TemplateDeletedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }
}
