namespace Logitar.Portal.Core2
{
  public abstract class DomainEvent
  {
    public AggregateId AggregateId { get; set; }
    public DateTime OccurredOn { get; set; }
    public AggregateId UserId { get; set; }
    public long Version { get; set; }
  }
}
