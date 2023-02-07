namespace Logitar.Portal.Contracts.Accounts
{
  public record AuthenticateWithGooglePayload
  {
    public string Credential { get; set; } = string.Empty;

    public bool IgnoreProviderLocale { get; set; }
    public string? Locale { get; set; }
  }
}
