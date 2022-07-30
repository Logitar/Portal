namespace Portal.Core.Accounts.Payloads
{
  public class ResetPasswordPayload
  {
    public string Realm { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string Password { get; set; } = null!;
  }
}
