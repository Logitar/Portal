using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Logging;

internal class LogEventEntity
{
  public LogEventEntity(EventEntity @event, LogEntity log, ActivityEntity? activity = null)
  {
    EventId = @event.EventId;

    Log = log;
    LogId = log.LogId;

    Activity = activity;
    ActivityId = activity?.ActivityId;
  }

  private LogEventEntity()
  {
  }

  public long EventId { get; private set; }

  public LogEntity? Log { get; private set; }
  public long LogId { get; private set; }

  public ActivityEntity? Activity { get; private set; }
  public long? ActivityId { get; private set; }
}
