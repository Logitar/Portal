using Logitar.EventSourcing;
using Logitar.Identity.Core;

namespace Logitar.Portal.Domain.Templates;

public readonly struct TemplateId
{
  private const char Separator = ':';

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public TenantId? TenantId { get; }
  public EntityId EntityId { get; }

  public TemplateId(TenantId? tenantId, EntityId entityId)
  {
    StreamId = new(tenantId == null ? entityId.Value : string.Join(Separator, tenantId, entityId));
    TenantId = tenantId;
    EntityId = entityId;
  }

  public TemplateId(StreamId streamId)
  {
    StreamId = streamId;

    string[] values = streamId.Value.Split(Separator);
    if (values.Length > 2)
    {
      throw new ArgumentException($"The value '{streamId}' is not a valid template ID.", nameof(streamId));
    }
    else if (values.Length == 2)
    {
      TenantId = new(values.First());
    }
    EntityId = new(values.Last());
  }

  public static TemplateId NewId(TenantId? tenantId = null) => new(tenantId, EntityId.NewId());

  public static bool operator ==(TemplateId left, TemplateId right) => left.Equals(right);
  public static bool operator !=(TemplateId left, TemplateId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is TemplateId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
