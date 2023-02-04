using MediatR;

namespace Logitar.Portal.Domain.Templates.Events
{
  public record TemplateUpdatedEvent : DomainEvent, INotification
  {
    public string? DisplayName { get; init; }
    public string? Description { get; init; }

    public string Subject { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public string Contents { get; init; } = string.Empty;
  }
}
