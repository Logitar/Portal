using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Portal.Domain.Settings.Validators;

namespace Logitar.Portal.Domain.Settings;

public record ReadOnlyPasswordSettings : IPasswordSettings
{
  public int RequiredLength { get; }
  public int RequiredUniqueChars { get; }
  public bool RequireNonAlphanumeric { get; }
  public bool RequireLowercase { get; }
  public bool RequireUppercase { get; }
  public bool RequireDigit { get; }
  public string HashingStrategy { get; }

  public ReadOnlyPasswordSettings() : this(requiredLength: 8, requiredUniqueChars: 8, requireNonAlphanumeric: true,
    requireLowercase: true, requireUppercase: true, requireDigit: true, hashingStrategy: "PBKDF2")
  {
  }

  public ReadOnlyPasswordSettings(IPasswordSettings settings) : this(settings.RequiredLength, settings.RequiredUniqueChars, settings.RequireNonAlphanumeric,
    settings.RequireLowercase, settings.RequireUppercase, settings.RequireDigit, settings.HashingStrategy)
  {
  }

  public ReadOnlyPasswordSettings(int requiredLength, int requiredUniqueChars, bool requireNonAlphanumeric,
    bool requireLowercase, bool requireUppercase, bool requireDigit, string hashingStrategy)
  {
    RequiredLength = requiredLength;
    RequiredUniqueChars = requiredUniqueChars;
    RequireNonAlphanumeric = requireNonAlphanumeric;
    RequireLowercase = requireLowercase;
    RequireUppercase = requireUppercase;
    RequireDigit = requireDigit;
    HashingStrategy = hashingStrategy;
    new PasswordSettingsValidator().ValidateAndThrow(this);
  }
}
