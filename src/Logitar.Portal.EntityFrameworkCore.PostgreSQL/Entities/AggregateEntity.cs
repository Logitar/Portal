using Logitar.EventSourcing;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;

internal abstract class AggregateEntity
{
  protected AggregateEntity()
  {
  }

  protected AggregateEntity(DomainEvent e, ActorEntity actor)
  {
    AggregateId = e.AggregateId.Value;
    SetVersion(e);

    CreatedById = e.ActorId.ToGuid();
    CreatedBy = actor.Serialize();
    CreatedOn = e.OccurredOn;
  }

  public string AggregateId { get; private set; } = string.Empty;
  public long Version { get; private set; }

  public Guid CreatedById { get; private set; }
  public string CreatedBy { get; private set; } = ActorEntity.System.Serialize();
  public DateTime CreatedOn { get; private set; }

  public Guid? UpdatedById { get; private set; }
  public string? UpdatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }

  public virtual void SetActor(Guid id, ActorEntity actor)
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

  protected void SetVersion(DomainEvent e) => Version = e.Version;

  protected void Update(DomainEvent e, ActorEntity actor)
  {
    SetVersion(e);

    UpdatedById = e.ActorId.ToGuid();
    UpdatedBy = actor.Serialize();
    UpdatedOn = e.OccurredOn;
  }
}
