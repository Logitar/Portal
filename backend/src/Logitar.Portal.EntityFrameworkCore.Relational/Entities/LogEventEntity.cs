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
    EventId = @event.Id.ToGuid();
  }

  private LogEventEntity()
  {
  }

  public override bool Equals(object? obj) => obj is LogEventEntity log && log.LogId == LogId && log.EventId == EventId;
  public override int GetHashCode() => HashCode.Combine(LogId, EventId);
  public override string ToString() => $"{GetType()} (LogId={LogId}, EventId={EventId})";
}
