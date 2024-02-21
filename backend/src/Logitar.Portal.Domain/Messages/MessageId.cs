using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Domain.Messages;

public record MessageId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public MessageId(Guid id, string? propertyName = null) : this(new AggregateId(id), propertyName)
  {
  }

  public MessageId(AggregateId aggregateId, string? propertyName = null)
  {
    new IdValidator(propertyName).ValidateAndThrow(aggregateId.Value);

    AggregateId = aggregateId;
  }

  public MessageId(string value, string? propertyName = null)
  {
    value = value.Trim();
    new IdValidator(propertyName).ValidateAndThrow(value);

    AggregateId = new(value);
  }

  public static MessageId NewId() => new(AggregateId.NewId());

  public static MessageId? TryCreate(string? value, string? propertyName = null)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value, propertyName);
  }

  public Guid ToGuid() => AggregateId.ToGuid();
}
