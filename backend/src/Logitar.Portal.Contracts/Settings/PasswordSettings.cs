using Logitar.Identity.Contracts.Settings;

namespace Logitar.Portal.Contracts.Settings;

public record PasswordSettings : IPasswordSettings
{
  public int RequiredLength { get; set; } = 8;
  public int RequiredUniqueChars { get; set; } = 8;
  public bool RequireNonAlphanumeric { get; set; } = true;
  public bool RequireLowercase { get; set; } = true;
  public bool RequireUppercase { get; set; } = true;
  public bool RequireDigit { get; set; } = true;
  public string HashingStrategy { get; set; } = "PBKDF2";

  public PasswordSettings() : this(requiredLength: 8, requiredUniqueChars: 8, requireNonAlphanumeric: true,
    requireLowercase: true, requireUppercase: true, requireDigit: true, hashingStrategy: "PBKDF2")
  {
  }

  public PasswordSettings(IPasswordSettings settings) : this(settings.RequiredLength, settings.RequiredUniqueChars, settings.RequireNonAlphanumeric,
    settings.RequireLowercase, settings.RequireUppercase, settings.RequireDigit, settings.HashingStrategy)
  {
  }

  public PasswordSettings(int requiredLength, int requiredUniqueChars, bool requireNonAlphanumeric,
    bool requireLowercase, bool requireUppercase, bool requireDigit, string hashingStrategy)
  {
    RequiredLength = requiredLength;
    RequiredUniqueChars = requiredUniqueChars;
    RequireNonAlphanumeric = requireNonAlphanumeric;
    RequireLowercase = requireLowercase;
    RequireUppercase = requireUppercase;
    RequireDigit = requireDigit;
    HashingStrategy = hashingStrategy;
  }
}
