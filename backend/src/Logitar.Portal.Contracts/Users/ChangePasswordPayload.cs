namespace Logitar.Portal.Contracts.Users;

public record ChangePasswordPayload
{
  public string? CurrentPassword { get; set; }
  public string NewPassword { get; set; }

  public ChangePasswordPayload() : this(currentPassword: null, newPassword: string.Empty)
  {
  }

  public ChangePasswordPayload(string? currentPassword, string newPassword)
  {
    CurrentPassword = currentPassword;
    NewPassword = newPassword;
  }
}
