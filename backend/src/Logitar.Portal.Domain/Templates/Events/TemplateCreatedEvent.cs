using MediatR;

namespace Logitar.Portal.Domain.Templates.Events
{
  public record TemplateCreatedEvent : DomainEvent, INotification
  {
    public AggregateId? RealmId { get; set; }

    public string Key { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }

    public string Subject { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Contents { get; set; } = string.Empty;
  }
}
