using Logitar.Identity.Contracts.Settings;

namespace Logitar.Portal.Contracts.Realms;

public record PasswordSettings : IPasswordSettings
{
  public int RequiredLength { get; set; } = 8;
  public int RequiredUniqueChars { get; set; } = 8;
  public bool RequireNonAlphanumeric { get; set; } = true;
  public bool RequireLowercase { get; set; } = true;
  public bool RequireUppercase { get; set; } = true;
  public bool RequireDigit { get; set; } = true;
  public string HashingStrategy { get; set; } = "PBKDF2";
}
