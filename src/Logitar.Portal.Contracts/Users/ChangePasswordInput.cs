namespace Logitar.Portal.Contracts.Users;

public record ChangePasswordInput
{
  public string Current { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}
