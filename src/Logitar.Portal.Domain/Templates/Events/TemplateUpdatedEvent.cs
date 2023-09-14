using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using MediatR;

namespace Logitar.Portal.Domain.Templates.Events;

public record TemplateUpdatedEvent : DomainEvent, INotification
{
  public string? UniqueName { get; set; }
  public Modification<string>? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public string? Subject { get; set; }
  public string? ContentType { get; set; }
  public string? Contents { get; set; }
}
