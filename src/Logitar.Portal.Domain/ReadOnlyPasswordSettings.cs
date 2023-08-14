using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Settings;

namespace Logitar.Portal.Domain;

public record ReadOnlyPasswordSettings : IPasswordSettings
{
  public int RequiredLength { get; init; } = 8;
  public int RequiredUniqueChars { get; init; } = 8;
  public bool RequireNonAlphanumeric { get; init; } = true;
  public bool RequireLowercase { get; init; } = true;
  public bool RequireUppercase { get; init; } = true;
  public bool RequireDigit { get; init; } = true;

  public string Strategy { get; init; } = Pbkdf2.Prefix;
}
