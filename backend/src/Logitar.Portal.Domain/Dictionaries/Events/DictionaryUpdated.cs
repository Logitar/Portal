using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Portal.Domain.Dictionaries.Events;

public record DictionaryUpdated : DomainEvent, INotification
{
  public Dictionary<Identifier, string?> Entries { get; init; } = [];

  [JsonIgnore]
  public bool HasChanges => Entries.Count > 0;
}
