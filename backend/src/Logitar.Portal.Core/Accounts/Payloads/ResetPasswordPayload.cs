namespace Logitar.Portal.Core.Accounts.Payloads
{
  public class ResetPasswordPayload
  {
    public string Token { get; set; } = null!;
    public string Password { get; set; } = null!;
  }
}
