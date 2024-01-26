using FluentValidation;
using Logitar.Identity.Domain;

namespace Logitar.Portal.Domain.Realms.Validators;

internal class SlugValidator : AbstractValidator<string>
{
  public SlugValidator(string? propertyName = null)
  {
    When(x => !string.IsNullOrWhiteSpace(x), () => RuleFor(x => x).Must(BeAValidSlug)
      .WithErrorCode(nameof(SlugValidator))
      .WithMessage("'{PropertyName}' must be composed of non-empty alphanumeric words separated by hyphens.")
      .WithPropertyName(propertyName)
    );
  }

  private static bool BeAValidSlug(string slug) => slug.Split('-').All(word => !string.IsNullOrEmpty(word) && word.All(char.IsLetterOrDigit));
}
