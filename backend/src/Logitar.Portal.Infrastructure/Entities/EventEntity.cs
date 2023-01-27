using Logitar.Portal.Core;
using System.Text.Json;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class EventEntity
  {
    private EventEntity()
    {
    }

    public long EventId { get; private set; }
    public long Version { get; private set; }

    public DateTime OccurredOn { get; private set; }
    public string UserId { get; private set; } = null!;

    public string AggregateType { get; private set; } = null!;
    public string AggregateId { get; private set; } = null!;

    public string EventType { get; private set; } = null!;
    public string EventData { get; private set; } = null!;

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
          UserId = change.UserId.ToString(),
          AggregateType = aggregateType,
          AggregateId = aggregate.Id.ToString(),
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
