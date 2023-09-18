namespace Logitar.Portal.Contracts.Users;

public record ResetPasswordPayload
{
  public string Realm { get; set; } = string.Empty;
  public string Token { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}
