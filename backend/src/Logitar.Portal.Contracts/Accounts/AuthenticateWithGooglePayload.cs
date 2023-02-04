using System.Globalization;

namespace Logitar.Portal.Contracts.Accounts
{
  public record AuthenticateWithGooglePayload
  {
    public string Credential { get; set; } = string.Empty;

    public bool IgnoreProviderLocale { get; set; }
    public CultureInfo? Locale { get; set; }
  }
}
