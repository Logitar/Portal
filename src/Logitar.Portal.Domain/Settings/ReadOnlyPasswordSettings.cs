using FluentValidation;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain.Settings.Validators;

namespace Logitar.Portal.Domain.Settings;

public record ReadOnlyPasswordSettings : IPasswordSettings
{
  public ReadOnlyPasswordSettings(int requiredLength = 8, int requiredUniqueChars = 8, bool requireNonAlphanumeric = true,
    bool requireLowercase = true, bool requireUppercase = true, bool requireDigit = true)
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
