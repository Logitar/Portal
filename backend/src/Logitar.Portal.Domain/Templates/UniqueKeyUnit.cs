using FluentValidation;
using Logitar.Portal.Domain.Templates.Validators;

namespace Logitar.Portal.Domain.Templates;

public record UniqueKeyUnit
{
  public string Value { get; }

  public UniqueKeyUnit(string value, string? propertyName = null)
  {
    Value = value.Trim();
    new UniqueKeyValidator(propertyName).ValidateAndThrow(Value);
  }

  public static UniqueKeyUnit? TryCreate(string? value, string? propertyName = null)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value, propertyName);
  }
}
