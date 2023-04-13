using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Core.Templates.Events;

public record TemplateDeleted : DomainEvent, INotification
{
  public TemplateDeleted() => DeleteAction = DeleteAction.Delete;
}
