using MediatR;

namespace Logitar.Portal.Domain.Templates.Events
{
  public record TemplateDeletedEvent : DomainEvent, INotification
  {
  }
}
