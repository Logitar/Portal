using Logitar.Portal.Domain;
using Logitar.Portal.Infrastructure.JsonConverters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class EventEntity
  {
    private static readonly JsonSerializerOptions _serializerOptions = new();

    static EventEntity()
    {
      _serializerOptions.Converters.Add(new AggregateIdConverter());
      _serializerOptions.Converters.Add(new CultureInfoConverter());
      _serializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    private EventEntity()
    {
    }

    public long EventId { get; private set; }
    public long Version { get; private set; }

    public string ActorId { get; private set; } = string.Empty;
    public DateTime OccurredOn { get; private set; }

    public string AggregateType { get; private set; } = string.Empty;
    public string AggregateId { get; private set; } = string.Empty;

    public string EventType { get; private set; } = string.Empty;
    public string EventData { get; private set; } = string.Empty;

    public static IEnumerable<EventEntity> FromChanges(AggregateRoot aggregate)
    {
      string aggregateType = aggregate.GetType().GetName();

      return aggregate.Changes.Select(change =>
      {
        Type eventType = change.GetType();

        return new EventEntity
        {
          Version = change.Version,
          ActorId = change.ActorId.Value,
          OccurredOn = change.OccurredOn,
          AggregateType = aggregateType,
          AggregateId = aggregate.Id.Value,
          EventType = eventType.GetName(),
          EventData = JsonSerializer.Serialize(change, eventType, _serializerOptions)
        };
      });
    }

    public DomainEvent Deserialize()
    {
      Type eventType = Type.GetType(EventType)
        ?? throw new InvalidOperationException($"The type '{EventType}' could not be resolved.");

      return (DomainEvent?)JsonSerializer.Deserialize(EventData, eventType, _serializerOptions)
        ?? throw new InvalidOperationException($"The event 'Id={EventId}' could not be deserialized.");
    }
  }
}
