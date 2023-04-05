namespace Logitar.Portal.Core.Accounts.Payloads
{
  public class AuthenticateWithGooglePayload
  {
    public string Credential { get; set; } = null!;

    public bool IgnoreProviderLocale { get; set; }
    public string? Locale { get; set; }
  }
}
