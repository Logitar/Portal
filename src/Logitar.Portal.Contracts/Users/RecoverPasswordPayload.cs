namespace Logitar.Portal.Contracts.Users;

public record RecoverPasswordPayload
{
  public string Realm { get; set; } = string.Empty;
  public string UniqueName { get; set; } = string.Empty;

  public bool IgnoreUserLocale { get; set; }
  public string? Locale { get; set; }
}
