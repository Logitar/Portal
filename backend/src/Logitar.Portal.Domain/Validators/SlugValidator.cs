using FluentValidation;
using Logitar.Identity.Domain;

namespace Logitar.Portal.Domain.Validators;

internal class SlugValidator : AbstractValidator<string>
{
  public SlugValidator(string? propertyName = null)
  {
    RuleFor(x => x).Must(BeAValidSlug).WithPropertyName(propertyName);
  }

  private static bool BeAValidSlug(string slug) => slug.Split('-').All(word => !string.IsNullOrEmpty(word) && word.All(char.IsLetterOrDigit));
}
