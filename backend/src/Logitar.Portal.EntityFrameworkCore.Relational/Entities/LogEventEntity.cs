using Logitar.EventSourcing;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal class LogEventEntity
{
  public LogEntity? Log { get; private set; }
  public long LogId { get; private set; }
  public string EventId { get; private set; } = string.Empty;

  public LogEventEntity(LogEntity log, DomainEvent @event)
  {
    Log = log;
    LogId = log.LogId;
    EventId = @event.Id.Value;
  }

  private LogEventEntity()
  {
  }
}
