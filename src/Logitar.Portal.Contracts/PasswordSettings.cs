namespace Logitar.Portal.Contracts;

public record PasswordSettings
{
  public int RequiredLength { get; set; } = 6;
  public int RequiredUniqueChars { get; set; } = 1;
  public bool RequireNonAlphanumeric { get; set; } = false;
  public bool RequireLowercase { get; set; } = true;
  public bool RequireUppercase { get; set; } = true;
  public bool RequireDigit { get; set; } = true;
}
