using System.Text;

namespace Logitar.Portal.Core2
{
  internal class EventNotSupportedException : NotSupportedException
  {
    public EventNotSupportedException(Type aggregateType, Type eventType)
      : base(GetMessage(aggregateType, eventType))
    {
      Data["AggregateType"] = aggregateType.GetName();
      Data["EventType"] = eventType.GetName();
    }

    private static string GetMessage(Type aggregateType, Type eventType)
    {
      StringBuilder message = new();

      message.AppendLine("The specified event is not supported by the aggregate.");
      message.AppendLine($"Aggregate type: {aggregateType.GetName()}");
      message.AppendLine($"Event type: {eventType.GetName()}");

      return message.ToString();
    }
  }
}
