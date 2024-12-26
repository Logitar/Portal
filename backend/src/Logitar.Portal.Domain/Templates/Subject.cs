using FluentValidation;
using Logitar.Portal.Domain.Templates.Validators;

namespace Logitar.Portal.Domain.Templates;

public record Subject
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public Subject(string value, string? propertyName = null)
  {
    Value = value.Trim();
    new SubjectValidator(propertyName).ValidateAndThrow(Value);
  }

  public static Subject? TryCreate(string? value, string? propertyName = null)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value, propertyName);
  }
}
