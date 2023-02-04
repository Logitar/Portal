using MediatR;

namespace Logitar.Portal.Domain.ApiKeys.Events
{
  public record ApiKeyDeletedEvent : DomainEvent, INotification
  {
  }
}
