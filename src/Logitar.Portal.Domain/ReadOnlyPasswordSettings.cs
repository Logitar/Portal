using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Settings;

namespace Logitar.Portal.Domain;

public record ReadOnlyPasswordSettings : IPasswordSettings
{
  public int RequiredLength { get; init; } = 6;
  public int RequiredUniqueChars { get; init; } = 1;
  public bool RequireNonAlphanumeric { get; init; } = false;
  public bool RequireLowercase { get; init; } = true;
  public bool RequireUppercase { get; init; } = true;
  public bool RequireDigit { get; init; } = true;

  public string Strategy { get; init; } = Pbkdf2.Prefix;
}
