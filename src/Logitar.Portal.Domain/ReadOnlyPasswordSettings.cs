﻿using FluentValidation;
using Logitar.Portal.Domain.Realms.Validators;

namespace Logitar.Portal.Domain;

public record class ReadOnlyPasswordSettings
{
  public ReadOnlyPasswordSettings(int requiredLength = 6, int requiredUniqueChars = 1,
    bool requireNonAlphanumeric = false, bool requireLowercase = true,
    bool requireUppercase = true, bool requireDigit = true)
  {
    RequiredLength = requiredLength;
    RequiredUniqueChars = requiredUniqueChars;
    RequireNonAlphanumeric = requireNonAlphanumeric;
    RequireLowercase = requireLowercase;
    RequireUppercase = requireUppercase;
    RequireDigit = requireDigit;

    new ReadOnlyPasswordSettingsValidator().ValidateAndThrow(this);
  }

  public int RequiredLength { get; }
  public int RequiredUniqueChars { get; }
  public bool RequireNonAlphanumeric { get; }
  public bool RequireLowercase { get; }
  public bool RequireUppercase { get; }
  public bool RequireDigit { get; }
}
