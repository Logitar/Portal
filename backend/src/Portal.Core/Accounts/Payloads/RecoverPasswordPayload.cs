namespace Portal.Core.Accounts.Payloads
{
  public class RecoverPasswordPayload
  {
    public string Realm { get; set; } = null!;
    public string Username { get; set; } = null!;
  }
}
