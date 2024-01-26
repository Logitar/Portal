namespace Logitar.Portal.Contracts.Users;

public record UpdateUserPayload
{
  public string? UniqueName { get; set; }
  public bool? IsDisabled { get; set; }
}
