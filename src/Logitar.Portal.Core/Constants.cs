namespace Logitar.Portal.Core;

public static class Constants
{
  public static class PasswordRecovery
  {
    public const int Lifetime = 7 * 24 * 60 * 60; // 7 days
    public const string Purpose = "reset_password";
  }

  public static class PortalRealm
  {
    public const string UniqueName = "portal";
    public const string DisplayName = "Portal";
    public const string Description = "The realm in which the administrator users of the Portal belong to.";
  }
}
