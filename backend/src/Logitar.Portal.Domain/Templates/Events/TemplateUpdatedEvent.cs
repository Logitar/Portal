using Logitar.EventSourcing;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Portal.Domain.Templates.Events;

public record TemplateUpdatedEvent : DomainEvent, INotification
{
  public Modification<DisplayNameUnit>? DisplayName { get; set; }
  public Modification<DescriptionUnit>? Description { get; set; }

  public SubjectUnit? Subject { get; set; }
  public ContentUnit? Content { get; set; }

  public bool HasChanges => DisplayName != null || Description != null
    || Subject != null || Content != null;
}
