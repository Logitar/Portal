using Logitar.EventSourcing;
using Logitar.Identity.Core;

namespace Logitar.Portal.Domain.Messages;

public readonly struct MessageId
{
  private const char Separator = ':';

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public TenantId? TenantId { get; }
  public EntityId EntityId { get; }

  public MessageId(TenantId? tenantId, EntityId entityId)
  {
    StreamId = new(tenantId == null ? entityId.Value : string.Join(Separator, tenantId, entityId));
    TenantId = tenantId;
    EntityId = entityId;
  }

  public MessageId(StreamId streamId)
  {
    StreamId = streamId;

    string[] values = streamId.Value.Split(Separator);
    if (values.Length > 2)
    {
      throw new ArgumentException($"The value '{streamId}' is not a valid message ID.", nameof(streamId));
    }
    else if (values.Length == 2)
    {
      TenantId = new(values.First());
    }
    EntityId = new(values.Last());
  }

  public static MessageId NewId(TenantId? tenantId = null) => new(tenantId, EntityId.NewId());

  public static bool operator ==(MessageId left, MessageId right) => left.Equals(right);
  public static bool operator !=(MessageId left, MessageId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is MessageId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
