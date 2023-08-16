using FluentValidation;
using Logitar.Identity.Domain.Validators;

namespace Logitar.Portal.Domain.Realms.Validators;

internal class ClaimMappingKeyValidator : AbstractValidator<string>
{
  public ClaimMappingKeyValidator(string? propertyName = null)
  {
    IRuleBuilderOptions<string, string> options = RuleFor(x => x).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();

    if (propertyName != null)
    {
      options = options.WithName(propertyName);
    }
  }
}
