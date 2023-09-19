using Logitar.EventSourcing.EntityFrameworkCore.Relational;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record LogEventEntity
{
  public LogEventEntity(LogEntity log, EventEntity @event)
  {
    Log = log;
    LogId = log.LogId;
    EventId = @event.EventId;
  }
  private LogEventEntity()
  {
  }

  public LogEntity? Log { get; private set; }
  public long LogId { get; private set; }
  public long EventId { get; private set; }
}
