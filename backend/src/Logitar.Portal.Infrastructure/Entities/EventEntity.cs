using Logitar.Portal.Domain;
using System.Text.Json;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class EventEntity
  {
    private EventEntity()
    {
    }

    public long EventId { get; set; }
    public long Version { get; set; }

    public DateTime OccurredOn { get; set; }
    public string UserId { get; set; } = string.Empty;

    public string AggregateType { get; set; } = string.Empty;
    public string AggregateId { get; set; } = string.Empty;

    public string EventType { get; set; } = string.Empty;
    public string EventData { get; set; } = string.Empty;

    public static IEnumerable<EventEntity> FromChanges(AggregateRoot aggregate)
    {
      string aggregateType = aggregate.GetType().GetName();

      return aggregate.Changes.Select(change =>
      {
        Type eventType = change.GetType();

        return new EventEntity
        {
          Version = change.Version,
          OccurredOn = change.OccurredOn,
          UserId = change.UserId.Value,
          AggregateType = aggregateType,
          AggregateId = aggregate.Id.Value,
          EventType = eventType.GetName(),
          EventData = JsonSerializer.Serialize(change, eventType)
        };
      });
    }

    public DomainEvent Deserialize()
    {
      Type eventType = Type.GetType(EventType)
        ?? throw new InvalidOperationException($"The type '{EventType}' could not be resolved.");

      return (DomainEvent?)JsonSerializer.Deserialize(EventData, eventType)
        ?? throw new InvalidOperationException($"The event 'Id={EventId}' could not be deserialized.");
    }
  }
}
