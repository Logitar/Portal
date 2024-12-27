using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Portal.Domain.Templates.Events;

public record TemplateUpdated : DomainEvent, INotification
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  public Subject? Subject { get; set; }
  public Content? Content { get; set; }

  [JsonIgnore]
  public bool HasChanges => DisplayName != null || Description != null || Subject != null || Content != null;
}
