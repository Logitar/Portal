namespace Logitar.Portal.Infrastructure.Entities
{
  internal class LogEventEntity
  {
    public LogEventEntity(EventEntity @event, LogEntity log)
    {
      Event = @event;
      EventId = @event.EventId;

      Log = log;
      LogId = log.LogId;
    }
    private LogEventEntity()
    {
    }

    public EventEntity? Event { get; private set; }
    public long EventId { get; private set; }

    public LogEntity? Log { get; private set; }
    public long LogId { get; private set; }
  }
}
