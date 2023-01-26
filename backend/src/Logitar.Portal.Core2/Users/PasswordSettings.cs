namespace Logitar.Portal.Core2.Users
{
  public record PasswordSettings
  {
    public int RequiredLength { get; init; }
    public int RequiredUniqueChars { get; init; }
    public bool RequireNonAlphanumeric { get; init; }
    public bool RequireLowercase { get; init; }
    public bool RequireUppercase { get; init; }
    public bool RequireDigit { get; init; }
  }
}
