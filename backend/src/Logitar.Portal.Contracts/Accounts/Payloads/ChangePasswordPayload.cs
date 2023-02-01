namespace Logitar.Portal.Contracts.Accounts.Payloads
{
  public class ChangePasswordPayload
  {
    public string Current { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
  }
}
