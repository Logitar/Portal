using FluentValidation;
using FluentValidation.Validators;

namespace Logitar.Portal.Domain.Validators;

internal class ContentTypeValidator<T> : IPropertyValidator<T, string>
{
  private readonly HashSet<string> _contentTypes = [MediaTypeNames.Text.Html, MediaTypeNames.Text.Plain];

  public IReadOnlyCollection<string> ContentTypes => [.. _contentTypes];
  public string Name { get; } = "ContentTypeValidator";

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return $"'{{PropertyName}}' must be one of the following: {string.Join(", ", _contentTypes)}.";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    return string.IsNullOrEmpty(value) || _contentTypes.Contains(value);
  }
}
