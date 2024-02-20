using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Domain.Senders;

public record SenderId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public SenderId(AggregateId aggregateId, string? propertyName = null)
  {
    new IdValidator(propertyName).ValidateAndThrow(aggregateId.Value);

    AggregateId = aggregateId;
  }

  public SenderId(string value, string? propertyName = null)
  {
    value = value.Trim();
    new IdValidator(propertyName).ValidateAndThrow(value);

    AggregateId = new(value);
  }

  public static SenderId NewId() => new(AggregateId.NewId());

  public static SenderId? TryCreate(string? value, string? propertyName = null)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value, propertyName);
  }
}
