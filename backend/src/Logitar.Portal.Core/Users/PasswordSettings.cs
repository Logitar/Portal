namespace Logitar.Portal.Core.Users
{
  public class PasswordSettings
  {
    public PasswordSettings(int requiredLength, int requiredUniqueChars,
      bool requireNonAlphanumeric, bool requireLowercase, bool requireUppercase, bool requireDigit)
    {
      RequireDigit = requireDigit;
      RequireLowercase = requireLowercase;
      RequireNonAlphanumeric = requireNonAlphanumeric;
      RequireUppercase = requireUppercase;
      RequiredLength = requiredLength;
      RequiredUniqueChars = requiredUniqueChars;
    }

    public int RequiredLength { get; private set; }
    public int RequiredUniqueChars { get; private set; }
    public bool RequireNonAlphanumeric { get; private set; }
    public bool RequireLowercase { get; private set; }
    public bool RequireUppercase { get; private set; }
    public bool RequireDigit { get; private set; }
  }
}
