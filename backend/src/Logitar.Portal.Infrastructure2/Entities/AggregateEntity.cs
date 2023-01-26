using Logitar.Portal.Core2;

namespace Logitar.Portal.Infrastructure2.Entities
{
  internal abstract class AggregateEntity
  {
    protected AggregateEntity()
    {
    }
    protected AggregateEntity(DomainEvent @event)
    {
      AggregateId = @event.AggregateId.ToString();
      Version = @event.Version;

      CreatedBy = @event.UserId.ToString();
      CreatedOn = @event.OccurredOn;
    }

    public string AggregateId { get; private set; } = null!;
    public long Version { get; private set; }

    public string CreatedBy { get; private set; } = null!;
    public DateTime CreatedOn { get; private set; }

    public string? UpdatedBy { get; private set; }
    public DateTime? UpdatedOn { get; private set; }

    protected void Update(DomainEvent @event)
    {
      Version = @event.Version;

      UpdatedBy = @event.UserId.ToString();
      UpdatedOn = @event.OccurredOn;
    }
  }
}
