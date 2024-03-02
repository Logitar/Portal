using Logitar.EventSourcing;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal class LogEntity
{
  public long LogId { get; private set; }
  public Guid UniqueId { get; private set; }

  public string? CorrelationId { get; private set; }

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

  public LogEntity(string? correlationId)
  {
    UniqueId = Guid.NewGuid();

    CorrelationId = correlationId;
  }

  private LogEntity()
  {
  }
}
