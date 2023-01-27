using FluentValidation;

namespace Logitar.Portal.Core2.Users
{
  internal class PasswordSettingsValidator : AbstractValidator<PasswordSettings>
  {
    public PasswordSettingsValidator()
    {
      RuleFor(x => x.RequiredLength).GreaterThanOrEqualTo(0);

      RuleFor(x => x.RequiredUniqueChars)
        .GreaterThanOrEqualTo(0)
        .LessThanOrEqualTo(x => x.RequiredLength);
    }
  }
}
