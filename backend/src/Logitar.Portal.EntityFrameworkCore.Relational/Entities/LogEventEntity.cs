using Logitar.EventSourcing;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal class LogEventEntity
{
  public LogEntity? Log { get; private set; }
  public long LogId { get; private set; }

  public Guid EventId { get; private set; }

  public LogEventEntity(LogEntity log, DomainEvent @event)
  {
    Log = log;
    LogId = log.LogId;

    EventId = @event.Id;
  }

  private LogEventEntity()
  {
  }
}
