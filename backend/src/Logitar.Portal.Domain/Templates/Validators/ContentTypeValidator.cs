using FluentValidation;
using Logitar.Identity.Domain;

namespace Logitar.Portal.Domain.Templates.Validators;

internal class ContentTypeValidator : AbstractValidator<string>
{
  private static readonly HashSet<string> _contentTypes = new([MediaTypeNames.Text.Html, MediaTypeNames.Text.Plain]);

  public ContentTypeValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .Must(_contentTypes.Contains)
        .WithErrorCode(nameof(ContentTypeValidator))
        .WithMessage($"'{{PropertyName}}' must be one of the following: {string.Join(", ", _contentTypes)}")
      .WithPropertyName(propertyName);
  }
}
