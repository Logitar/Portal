using MediatR;

namespace Logitar.Portal.Domain.ApiKeys.Events
{
  public record ApiKeyCreatedEvent : DomainEvent, INotification
  {
    public string SecretHash { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }

    public DateTime? ExpiresOn { get; init; }
  }
}
