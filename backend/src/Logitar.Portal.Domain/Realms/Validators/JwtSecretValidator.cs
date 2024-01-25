using FluentValidation;
using Logitar.Identity.Domain;

namespace Logitar.Portal.Domain.Realms.Validators;

internal class JwtSecretValidator : AbstractValidator<string>
{
  public JwtSecretValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(JwtSecretUnit.MaximumLength)
      .MinimumLength(JwtSecretUnit.MinimumLength)
      .WithPropertyName(propertyName);
  }
}
