using FluentValidation;
using Logitar.Identity.Domain;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Realms.Validators;

public class UniqueSlugValidator : AbstractValidator<string>
{
  public UniqueSlugValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(Slug.MaximumLength)
      .SetValidator(new SlugValidator(propertyName))
      .WithPropertyName(propertyName);
  }
}
