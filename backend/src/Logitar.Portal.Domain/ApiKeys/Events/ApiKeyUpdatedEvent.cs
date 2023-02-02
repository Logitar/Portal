using MediatR;

namespace Logitar.Portal.Domain.ApiKeys.Events
{
  public record ApiKeyUpdatedEvent : DomainEvent, INotification
  {
    public string DisplayName { get; init; } = string.Empty;
    public string? Description { get; init; }
  }
}
