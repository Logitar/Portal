using FluentValidation;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Settings.Validators;

internal class JwtSecretValidator : AbstractValidator<JwtSecret>
{
  public JwtSecretValidator(string? propertyName = null)
  {
    RuleFor(x => x.Value).NotEmpty()
      .MinimumLength(256 / 8).WithPropertyName(propertyName)
      .MaximumLength(512 / 8)
      .WithPropertyName(propertyName);
  }
}
