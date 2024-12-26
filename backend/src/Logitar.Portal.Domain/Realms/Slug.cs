using FluentValidation;
using Logitar.Portal.Domain.Realms.Validators;

namespace Logitar.Portal.Domain.Realms;

public record Slug
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public Slug(string value, string? propertyName = null)
  {
    Value = value.Trim();
    new UniqueSlugValidator(propertyName).ValidateAndThrow(value);
  }

  public static Slug? TryCreate(string? value, string? propertyName = null)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value, propertyName);
  }
}
