namespace Logitar.Portal.Contracts.Accounts
{
  public record RecoverPasswordPayload
  {
    public string Username { get; set; } = string.Empty;

    public bool IgnoreUserLocale { get; set; }
    public string? Locale { get; set; }
  }
}
