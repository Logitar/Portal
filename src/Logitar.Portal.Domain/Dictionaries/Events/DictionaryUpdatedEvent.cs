using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Dictionaries.Events;

public record DictionaryUpdatedEvent : DomainEvent, INotification
{
  public Dictionary<string, string?> Entries { get; init; } = new();
}
