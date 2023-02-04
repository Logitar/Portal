namespace Logitar.Portal.Application.Accounts
{
  internal static class ResetPassword
  {
    public const int Lifetime = 7 * 24 * 60 * 60; // 7 days
    public const string Purpose = "reset_password";
  }
}
