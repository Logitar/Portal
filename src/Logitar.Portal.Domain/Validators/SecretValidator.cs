using FluentValidation;

namespace Logitar.Portal.Domain.Validators;

internal class SecretValidator : AbstractValidator<string>
{
  public SecretValidator(string? propertyName = null)
  {
    IRuleBuilderOptions<string, string> options = RuleFor(x => x).NotEmpty()
      .MinimumLength(256 / 8)
      .MaximumLength(512 / 8);

    if (propertyName != null)
    {
      options = options.WithName(propertyName);
    }
  }
}
