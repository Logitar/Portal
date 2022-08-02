using FluentValidation;

namespace Logitar.Portal.Core.Users
{
  internal class PasswordSettingsValidator : AbstractValidator<PasswordSettings>
  {
    public PasswordSettingsValidator()
    {
      RuleFor(x => x.RequiredLength)
        .GreaterThan(0);

      RuleFor(x => x.RequiredUniqueChars)
        .GreaterThan(0)
        .LessThanOrEqualTo(x => x.RequiredLength);
    }
  }
}
