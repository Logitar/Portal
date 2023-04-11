using Logitar.EventSourcing;

namespace Logitar.Portal.v2.Core.Dictionaries.Events;

public abstract record DictionarySaved : DomainEvent
{
  public Dictionary<string, string> Entries { get; init; } = new();
}
