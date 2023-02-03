using MediatR;

namespace Logitar.Portal.Domain.Dictionaries.Events
{
  public record DictionaryDeletedEvent : DomainEvent, INotification
  {
  }
}
