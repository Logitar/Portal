using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Core.Dictionaries.Events;

public record DictionaryDeleted : DomainEvent, INotification
{
  public DictionaryDeleted() => DeleteAction = DeleteAction.Delete;
}
