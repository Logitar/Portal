using Logitar.EventSourcing;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal abstract record AggregateEntity
{
  protected AggregateEntity()
  {
  }
  protected AggregateEntity(DomainEvent change)
  {
    AggregateId = change.AggregateId.Value;

    CreatedBy = change.ActorId.Value;
    CreatedOn = change.OccurredOn.ToUniversalTime();

    Update(change);
  }

  public string AggregateId { get; private set; } = string.Empty;

  public string CreatedBy { get; private set; } = string.Empty;
  public DateTime CreatedOn { get; private set; }

  public string UpdatedBy { get; private set; } = string.Empty;
  public DateTime UpdatedOn { get; private set; }

  public long Version { get; private set; }

  public void Update(DomainEvent change)
  {
    UpdatedBy = change.ActorId.Value;
    UpdatedOn = change.OccurredOn.ToUniversalTime();

    Version = change.Version;
  }
}
