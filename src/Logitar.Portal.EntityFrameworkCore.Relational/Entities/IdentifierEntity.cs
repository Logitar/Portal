using Logitar.Portal.Domain.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal abstract record IdentifierEntity
{
  protected IdentifierEntity()
  {
  }
  protected IdentifierEntity(IdentifierSetEvent @event, string? tenantId)
  {
    Id = @event.Id;

    TenantId = tenantId;
    Key = @event.Key;

    CreatedBy = @event.ActorId.Value;
    CreatedOn = @event.OccurredOn;

    Update(@event);
  }

  public Guid Id { get; private set; }

  public string? TenantId { get; private set; }
  public string Key { get; private set; } = string.Empty;
  public string Value { get; private set; } = string.Empty;
  public string ValueNormalized
  {
    get => Value.ToUpper();
    private set { }
  }

  public string CreatedBy { get; private set; } = string.Empty;
  public DateTime CreatedOn { get; private set; }

  public string UpdatedBy { get; private set; } = string.Empty;
  public DateTime UpdatedOn { get; private set; }

  public long Version { get; private set; }

  protected void Update(IdentifierSetEvent @event)
  {
    Value = @event.Value;

    UpdatedBy = @event.ActorId.Value;
    UpdatedOn = @event.OccurredOn;

    Version++;
  }
}
