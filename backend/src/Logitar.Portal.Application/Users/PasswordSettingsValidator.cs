using FluentValidation;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Users
{
  internal class PasswordSettingsValidator : AbstractValidator<PasswordSettings>
  {
    public PasswordSettingsValidator()
    {
      RuleFor(x => x.RequiredLength).GreaterThanOrEqualTo(1);

      RuleFor(x => x.RequiredUniqueChars).GreaterThanOrEqualTo(1)
        .LessThanOrEqualTo(x => x.RequiredLength);
    }
  }
}
