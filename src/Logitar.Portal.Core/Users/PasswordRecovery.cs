namespace Logitar.Portal.Core.Users;

internal static class PasswordRecovery
{
  public const int Lifetime = 7 * 24 * 60 * 60; // 7 days
  public const string Purpose = "reset_password";
}
