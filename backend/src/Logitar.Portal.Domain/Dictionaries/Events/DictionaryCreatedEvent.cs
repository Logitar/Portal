using MediatR;

namespace Logitar.Portal.Domain.Dictionaries.Events
{
  public record DictionaryCreatedEvent : DomainEvent, INotification
  {
    public AggregateId? RealmId { get; init; }

    public string LocaleName { get; init; } = string.Empty;

    public Dictionary<string, string>? Entries { get; init; }
  }
}
