using Logitar.EventSourcing;

namespace Logitar.Portal.Core.Templates.Events;

public abstract record TemplateSaved : DomainEvent
{
  public string? DisplayName { get; init; }
  public string? Description { get; init; }

  public string Subject { get; init; } = string.Empty;
  public string ContentType { get; init; } = string.Empty;
  public string Contents { get; init; } = string.Empty;
}
