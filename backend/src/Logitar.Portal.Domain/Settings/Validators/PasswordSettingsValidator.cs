using FluentValidation;
using Logitar.Identity.Contracts.Settings;

namespace Logitar.Portal.Domain.Settings.Validators;

public class PasswordSettingsValidator : AbstractValidator<IPasswordSettings>
{
  public PasswordSettingsValidator()
  {
    RuleFor(x => x.RequiredLength).GreaterThanOrEqualTo(settings => GetMinimumLength(settings));

    RuleFor(x => x.RequiredUniqueChars).GreaterThanOrEqualTo(settings => GetMinimumLength(settings))
      .LessThanOrEqualTo(x => x.RequiredLength);

    RuleFor(x => x.HashingStrategy).NotEmpty().MaximumLength(byte.MaxValue);
  }

  private static int GetMinimumLength(IPasswordSettings settings)
  {
    int minimumLength = 0;
    if (settings.RequireNonAlphanumeric)
    {
      minimumLength++;
    }
    if (settings.RequireLowercase)
    {
      minimumLength++;
    }
    if (settings.RequireUppercase)
    {
      minimumLength++;
    }
    if (settings.RequireDigit)
    {
      minimumLength++;
    }
    return minimumLength < 1 ? 1 : minimumLength;
  }
}
