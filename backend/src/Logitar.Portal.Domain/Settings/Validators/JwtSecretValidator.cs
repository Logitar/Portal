using FluentValidation;
using Logitar.Identity.Domain;

namespace Logitar.Portal.Domain.Settings.Validators;

public class JwtSecretValidator : AbstractValidator<string>
{
  public JwtSecretValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(JwtSecret.MaximumLength)
      .MinimumLength(JwtSecret.MinimumLength)
      .WithPropertyName(propertyName);
  }
}
