using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Templates.Events;

public record TemplateCreatedEvent : DomainEvent, INotification
{
  public TemplateCreatedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public string? TenantId { get; init; }

  public string UniqueName { get; init; } = string.Empty;

  public string Subject { get; init; } = string.Empty;
  public string ContentType { get; init; } = string.Empty;
  public string Contents { get; init; } = string.Empty;
}
