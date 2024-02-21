using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Domain.Dictionaries;

public record DictionaryId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public DictionaryId(Guid id, string? propertyName = null) : this(new AggregateId(id), propertyName)
  {
  }

  public DictionaryId(AggregateId aggregateId, string? propertyName = null)
  {
    new IdValidator(propertyName).ValidateAndThrow(aggregateId.Value);

    AggregateId = aggregateId;
  }

  public DictionaryId(string value, string? propertyName = null)
  {
    value = value.Trim();
    new IdValidator(propertyName).ValidateAndThrow(value);

    AggregateId = new(value);
  }

  public static DictionaryId NewId() => new(AggregateId.NewId());

  public static DictionaryId? TryCreate(string? value, string? propertyName = null)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value, propertyName);
  }

  public Guid ToGuid() => AggregateId.ToGuid();
}
