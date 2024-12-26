using Logitar.EventSourcing;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Portal.Domain.Templates.Events;

public record TemplateUpdated : DomainEvent, INotification
{
  public Modification<DisplayNameUnit>? DisplayName { get; set; }
  public Modification<DescriptionUnit>? Description { get; set; }

  public Subject? Subject { get; set; }
  public Content? Content { get; set; }

  [JsonIgnore]
  public bool HasChanges => DisplayName != null || Description != null || Subject != null || Content != null;
}
