using Logitar.Portal.v2.Contracts.Realms;

namespace Logitar.Portal.v2.Core.Realms;

internal record ReadOnlyPasswordSettings
{
  public ReadOnlyPasswordSettings()
  {
  }
  public ReadOnlyPasswordSettings(PasswordSettings passwordSettings)
  {
    RequiredLength = passwordSettings.RequiredLength;
    RequiredUniqueChars = passwordSettings.RequiredUniqueChars;
    RequireNonAlphanumeric = passwordSettings.RequireNonAlphanumeric;
    RequireLowercase = passwordSettings.RequireLowercase;
    RequireUppercase = passwordSettings.RequireUppercase;
    RequireDigit = passwordSettings.RequireDigit;
  }

  public int RequiredLength { get; init; } = 6;
  public int RequiredUniqueChars { get; init; } = 1;
  public bool RequireNonAlphanumeric { get; init; } = false;
  public bool RequireLowercase { get; init; } = true;
  public bool RequireUppercase { get; init; } = true;
  public bool RequireDigit { get; init; } = true;
}
