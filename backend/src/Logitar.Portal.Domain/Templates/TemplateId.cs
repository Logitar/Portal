using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Domain.Templates;

public record TemplateId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public TemplateId(AggregateId aggregateId, string? propertyName = null)
  {
    new IdValidator(propertyName).ValidateAndThrow(aggregateId.Value);

    AggregateId = aggregateId;
  }

  public TemplateId(string value, string? propertyName = null)
  {
    value = value.Trim();
    new IdValidator(propertyName).ValidateAndThrow(value);

    AggregateId = new(value);
  }

  public static TemplateId NewId() => new(AggregateId.NewId());

  public static TemplateId? TryCreate(string? value, string? propertyName = null)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value, propertyName);
  }

  public Guid ToGuid() => AggregateId.ToGuid();
}
