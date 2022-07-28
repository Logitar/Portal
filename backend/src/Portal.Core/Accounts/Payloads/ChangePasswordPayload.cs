namespace Portal.Core.Accounts.Payloads
{
  public class ChangePasswordPayload
  {
    public string Current { get; set; } = null!;
    public string Password { get; set; } = null!;
  }
}
