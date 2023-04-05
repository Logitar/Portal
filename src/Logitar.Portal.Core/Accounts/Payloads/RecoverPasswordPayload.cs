namespace Logitar.Portal.Core.Accounts.Payloads
{
  public class RecoverPasswordPayload
  {
    public string Username { get; set; } = null!;

    public bool IgnoreUserLocale { get; set; }
    public string? Locale { get; set; }
  }
}
