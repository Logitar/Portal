namespace Logitar.Portal.Contracts.Users;

public record RecoverPasswordInput
{
  public string Realm { get; set; } = string.Empty;
  public string Username { get; set; } = string.Empty;

  public bool IgnoreUserLocale { get; set; }
  public string? Locale { get; set; }
}
