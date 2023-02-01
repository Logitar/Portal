namespace Logitar.Portal.Contracts.Accounts.Payloads
{
  public class ResetPasswordPayload
  {
    public string Token { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
  }
}
