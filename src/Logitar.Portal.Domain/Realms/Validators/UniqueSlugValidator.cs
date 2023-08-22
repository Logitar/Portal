using FluentValidation;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Realms.Validators;

internal class UniqueSlugValidator : AbstractValidator<string>
{
  public UniqueSlugValidator(string? propertyName = null)
  {
    IRuleBuilderOptions<string, string> options = RuleFor(x => x).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Slug()
      .WithPropertyName(propertyName);
  }
}
