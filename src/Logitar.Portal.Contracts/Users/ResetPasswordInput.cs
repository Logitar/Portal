namespace Logitar.Portal.Contracts.Users;

public record ResetPasswordInput
{
  public string Token { get; set; } = string.Empty;

  public string Realm { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}
