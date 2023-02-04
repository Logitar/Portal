using Logitar.Portal.Domain;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal abstract class AggregateEntity
  {
    protected AggregateEntity()
    {
    }
    protected AggregateEntity(DomainEvent @event)
    {
      AggregateId = @event.AggregateId.Value;
      Version = @event.Version;

      CreatedBy = @event.UserId.Value;
      CreatedOn = @event.OccurredOn;
    }

    public string AggregateId { get; private set; } = string.Empty;
    public long Version { get; private set; }

    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime CreatedOn { get; private set; }

    public string? UpdatedBy { get; private set; }
    public DateTime? UpdatedOn { get; private set; }

    protected virtual void Update(DomainEvent @event)
    {
      Version = @event.Version;

      UpdatedBy = @event.UserId.Value;
      UpdatedOn = @event.OccurredOn;
    }
  }
}
