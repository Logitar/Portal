using FluentValidation;
using Logitar.Portal.Domain.Templates.Validators;

namespace Logitar.Portal.Domain.Templates;

public record UniqueKey
{
  public string Value { get; }

  public UniqueKey(string value, string? propertyName = null)
  {
    Value = value.Trim();
    new UniqueKeyValidator(propertyName).ValidateAndThrow(Value);
  }

  public static UniqueKey? TryCreate(string? value, string? propertyName = null)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value, propertyName);
  }
}
