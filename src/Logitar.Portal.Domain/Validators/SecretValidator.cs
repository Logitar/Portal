using FluentValidation;

namespace Logitar.Portal.Domain.Validators;

internal class SecretValidator : AbstractValidator<string>
{
  public SecretValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MinimumLength(256 / 8)
      .MaximumLength(512 / 8)
      .WithPropertyName(propertyName);
  }
}
