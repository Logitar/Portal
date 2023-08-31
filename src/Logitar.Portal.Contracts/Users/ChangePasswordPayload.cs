namespace Logitar.Portal.Contracts.Users;

public record ChangePasswordPayload
{
  public string? CurrentPassword { get; set; }
  public string NewPassword { get; set; } = string.Empty;
}
