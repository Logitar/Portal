using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Dictionaries.Events;

public record DictionaryUpdated : DomainEvent, INotification
{
  public Dictionary<string, string?> Entries { get; init; } = [];

  [JsonIgnore]
  public bool HasChanges => Entries.Count > 0;
}
