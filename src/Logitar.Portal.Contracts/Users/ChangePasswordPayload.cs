namespace Logitar.Portal.Contracts.Users;

public record ChangePasswordPayload
{
  public string? Current { get; set; }
  public string Password { get; set; } = string.Empty;
}
