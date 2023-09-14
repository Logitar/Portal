using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Messages.Events;

public record MessageCreatedEvent : DomainEvent, INotification
{
  public MessageCreatedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public string Subject { get; init; } = string.Empty;
  public string Body { get; init; } = string.Empty;

  public Recipients Recipients { get; init; } = new(); // TODO(fpion): converter?

  public AggregateId? RealmId { get; init; }
  public AggregateId SenderId { get; init; }
  public AggregateId TemplateId { get; init; }

  public bool IgnoreUserLocale { get; init; }
  public Locale? Locale { get; init; }

  public Variables Variables { get; init; } = new(); // TODO(fpion): converter?

  public bool IsDemo { get; init; }
}
