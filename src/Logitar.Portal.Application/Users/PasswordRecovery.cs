namespace Logitar.Portal.Application.Users;

internal static class PasswordRecovery
{
  public const int Lifetime = 30 * 60; // 30 minutes
  public const string TokenKey = "Token";
  public const string TokenType = "resetpassword+jwt";
}
