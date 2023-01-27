using FluentValidation;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Realms
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
