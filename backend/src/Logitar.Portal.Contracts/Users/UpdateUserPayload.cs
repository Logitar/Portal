namespace Logitar.Portal.Contracts.Users;

public record UpdateUserPayload
{
  public string? UniqueName { get; set; }
  public ChangePasswordPayload? Password { get; set; }
  public bool? IsDisabled { get; set; }
}
