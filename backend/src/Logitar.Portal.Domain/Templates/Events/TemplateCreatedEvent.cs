using MediatR;

namespace Logitar.Portal.Domain.Templates.Events
{
  public record TemplateCreatedEvent : DomainEvent, INotification
  {
    public AggregateId? RealmId { get; init; }

    public string Key { get; init; } = string.Empty;
    public string? DisplayName { get; init; }
    public string? Description { get; init; }

    public string Subject { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public string Contents { get; init; } = string.Empty;
  }
}
