namespace Logitar.Portal.Domain
{
  public abstract record DomainEvent
  {
    public AggregateId AggregateId { get; set; }
    public long Version { get; set; }

    public AggregateId ActorId { get; set; }
    public DateTime OccurredOn { get; set; }
  }
}
