using Logitar.EventSourcing;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal class LogEntity
{
  public long LogId { get; private set; }
  public Guid UniqueId { get; private set; }

  public string? CorrelationId { get; private set; }

  public int? StatusCode { get; private set; }
  public bool IsCompleted
  {
    get => StatusCode.HasValue;
    private set { }
  }
  public void Complete(int statusCode, DateTime? endedOn = null)
  {
    StatusCode = statusCode;
    EndedOn = (endedOn ?? DateTime.Now).ToUniversalTime();
  }

  public DateTime StartedOn { get; private set; }
  public DateTime? EndedOn { get; private set; }
  public TimeSpan? Duration
  {
    get => EndedOn.HasValue ? EndedOn.Value - StartedOn : null;
    private set { }
  }

  public List<LogEventEntity> Events { get; private set; } = [];
  public void AddEvent(DomainEvent @event)
  {
    Events.Add(new LogEventEntity(this, @event));
  }
  public void AddEvents(IEnumerable<DomainEvent> events)
  {
    foreach (DomainEvent @event in events)
    {
      AddEvent(@event);
    }
  }

  public LogEntity(string? correlationId, DateTime? startedOn = null)
  {
    UniqueId = Guid.NewGuid();

    CorrelationId = correlationId;

    StartedOn = (startedOn ?? DateTime.Now).ToUniversalTime();
  }

  private LogEntity()
  {
  }
}
