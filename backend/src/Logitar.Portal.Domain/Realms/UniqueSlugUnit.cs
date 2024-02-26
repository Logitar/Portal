using FluentValidation;
using Logitar.Portal.Domain.Realms.Validators;

namespace Logitar.Portal.Domain.Realms;

public record UniqueSlugUnit
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public UniqueSlugUnit(string value, string? propertyName = null)
  {
    Value = value.Trim();
    new UniqueSlugValidator(propertyName).ValidateAndThrow(value);
  }

  public static UniqueSlugUnit? TryCreate(string? value, string? propertyName = null)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value, propertyName);
  }
}
