using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Domain.Realms.Validators;

namespace Logitar.Portal.Domain.Realms;

public record ReadOnlyPasswordSettings : IPasswordSettings
{
  public int RequiredLength { get; }
  public int RequiredUniqueChars { get; }
  public bool RequireNonAlphanumeric { get; }
  public bool RequireLowercase { get; }
  public bool RequireUppercase { get; }
  public bool RequireDigit { get; }
  public string HashingStrategy { get; }

  public ReadOnlyPasswordSettings() : this(new PasswordSettings())
  {
  }

  public ReadOnlyPasswordSettings(IPasswordSettings password) : this(password.RequiredLength, password.RequiredUniqueChars, password.RequireNonAlphanumeric,
    password.RequireLowercase, password.RequireUppercase, password.RequireDigit, password.HashingStrategy)
  {
  }

  public ReadOnlyPasswordSettings(int requiredLength, int requiredUniqueChars, bool requireNonAlphanumeric, bool requireLowercase, bool requireUppercase,
    bool requireDigit, string hashingStrategy)
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
