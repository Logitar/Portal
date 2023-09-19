namespace Logitar.Portal.Contracts.Users;

public record AuthenticateUserPayload
{
  public string? Realm { get; set; }
  public string UniqueName { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}
