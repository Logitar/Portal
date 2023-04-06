namespace Logitar.Portal.v2.Core.Realms;

internal record ReadOnlyPasswordSettings(int RequiredLength = 6,
  int RequiredUniqueChars = 1,
  bool RequireNonAlphanumeric = false,
  bool RequireLowercase = true,
  bool RequireUppercase = true,
  bool RequireDigit = true);
