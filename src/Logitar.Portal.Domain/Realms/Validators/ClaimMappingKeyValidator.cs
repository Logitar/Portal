using FluentValidation;
using Logitar.Identity.Domain.Validators;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Realms.Validators;

internal class ClaimMappingKeyValidator : AbstractValidator<string>
{
  public ClaimMappingKeyValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier()
      .WithPropertyName(propertyName);
  }
}
