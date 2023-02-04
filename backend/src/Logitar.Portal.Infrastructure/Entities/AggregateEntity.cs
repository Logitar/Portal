using Logitar.Portal.Domain;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal abstract class AggregateEntity
  {
    protected AggregateEntity()
    {
    }
    protected AggregateEntity(DomainEvent @event, Actor actor)
    {
      AggregateId = @event.AggregateId.Value;
      Version = @event.Version;

      CreatedById = @event.ActorId.Value;
      CreatedBy = actor.Serialize();
      CreatedOn = @event.OccurredOn;
    }

    public string AggregateId { get; private set; } = string.Empty;
    public long Version { get; private set; }

    public string CreatedById { get; private set; } = string.Empty;
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime CreatedOn { get; private set; }

    public string? UpdatedById { get; private set; }
    public string? UpdatedBy { get; private set; }
    public DateTime? UpdatedOn { get; private set; }

    public virtual void UpdateActors(string id, Actor actor)
    {
      if (CreatedById == id)
      {
        CreatedBy = actor.Serialize();
      }

      if (UpdatedById == id)
      {
        UpdatedBy = actor.Serialize();
      }
    }

    protected virtual void Update(DomainEvent @event, Actor actor)
    {
      Version = @event.Version;

      UpdatedById = @event.ActorId.Value;
      UpdatedBy = actor.Serialize();
      UpdatedOn = @event.OccurredOn;
    }
  }
}
