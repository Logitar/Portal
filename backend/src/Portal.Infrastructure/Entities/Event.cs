using Portal.Core;
using System.Text.Json;

namespace Portal.Infrastructure.Entities
{
  internal class EventEntity
  {
    private EventEntity()
    {
    }

    public Guid Id { get; private set; }
    public long Sid { get; private set; }

    public DateTime OccurredAt { get; private set; }
    public Guid UserId { get; private set; }

    public string EventType { get; private set; } = null!;
    public string EventData { get; private set; } = null!;

    public string AggregateType { get; private set; } = null!;
    public Guid AggregateId { get; private set; }

    public static IEnumerable<EventEntity> FromChanges(Aggregate aggregate)
    {
      string aggregateType = aggregate?.GetType().GetName() ?? throw new ArgumentNullException(nameof(aggregate));

      return aggregate.Changes.Select(change =>
      {
        Type eventType = change?.GetType() ?? throw new ArgumentException($"The change collection cannot contain a null element.", nameof(aggregate));

        return new EventEntity
        {
          OccurredAt = change.OccurredAt,
          UserId = change.UserId,
          EventType = eventType.GetName(),
          EventData = JsonSerializer.Serialize(change, eventType),
          AggregateType = aggregateType,
          AggregateId = aggregate.Id
        };
      });
    }
  }
}
