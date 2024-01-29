﻿using FluentValidation;
using Logitar.Identity.Contracts.Settings;

namespace Logitar.Portal.Domain.Settings.Validators;

public class PasswordSettingsValidator : AbstractValidator<IPasswordSettings>
{
  public PasswordSettingsValidator()
  {
    RuleFor(x => x.RequiredLength).GreaterThanOrEqualTo(x => GetRequiredLength(x));
    RuleFor(x => x.RequiredUniqueChars).GreaterThanOrEqualTo(x => x.RequiredLength);
    RuleFor(x => x.HashingStrategy).NotEmpty().MaximumLength(byte.MaxValue);
  }

  private static int GetRequiredLength(IPasswordSettings settings)
  {
    int requiredLength = 0;
    if (settings.RequireNonAlphanumeric)
    {
      requiredLength++;
    }
    if (settings.RequireLowercase)
    {
      requiredLength++;
    }
    if (settings.RequireUppercase)
    {
      requiredLength++;
    }
    if (settings.RequireDigit)
    {
      requiredLength++;
    }
    return requiredLength < 1 ? 1 : requiredLength;
  }
}
