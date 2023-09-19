using FluentValidation;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Templates.Validators;

internal class ContentTypeValidator : AbstractValidator<string>
{
  private readonly HashSet<string> _allowedContentTypes = new(new[]
  {
    MediaTypeNames.Text.Html,
    MediaTypeNames.Text.Plain
  });

  public ContentTypeValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .Must(_allowedContentTypes.Contains)
      .WithErrorCode(nameof(ContentTypeValidator))
      .WithMessage($"'{{PropertyName}}' must be one of the following: {string.Join(", ", _allowedContentTypes)}")
      .WithPropertyName(propertyName);
  }
}
