using Logitar.EventSourcing;

namespace Logitar.Portal.Domain.Sessions.Events;

public record SessionUpdatedEvent : DomainEvent
{
  public Dictionary<string, string?> CustomAttributes { get; init; } = new();
}
