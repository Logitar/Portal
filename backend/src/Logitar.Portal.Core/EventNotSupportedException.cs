using System.Text;

namespace Logitar.Portal.Core
{
  internal class EventNotSupportedException : NotSupportedException
  {
    public EventNotSupportedException(Type aggregateType, Type eventType)
      : base(GetMessage(aggregateType, eventType))
    {
      AggregateType = aggregateType ?? throw new ArgumentNullException(nameof(aggregateType));
      EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
    }

    public Type AggregateType { get; }
    public Type EventType { get; }

    private static string GetMessage(Type aggregateType, Type eventType)
    {
      var message = new StringBuilder();

      message.AppendLine("The specified event is not supported by the aggregate.");
      message.AppendLine($"Aggregate type: {aggregateType?.GetName()}");
      message.AppendLine($"Event type: {eventType?.GetName()}");

      return message.ToString();
    }
  }
}
