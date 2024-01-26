using FluentValidation;
using Logitar.Identity.Domain;

namespace Logitar.Portal.Domain.Realms.Validators;

internal class UniqueSlugValidator : AbstractValidator<string>
{
  public UniqueSlugValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(UniqueSlugUnit.MaximumLength)
      .SetValidator(new SlugValidator(propertyName))
      .WithPropertyName(propertyName);
  }
}
