using MediatR;
using System.Globalization;

namespace Logitar.Portal.Domain.Dictionaries.Events
{
  public record DictionaryCreatedEvent : DomainEvent, INotification
  {
    public AggregateId? RealmId { get; init; }

    public CultureInfo Locale { get; init; } = CultureInfo.InvariantCulture;

    public Dictionary<string, string>? Entries { get; init; }
  }
}
