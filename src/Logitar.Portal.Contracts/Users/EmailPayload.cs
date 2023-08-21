namespace Logitar.Portal.Contracts.Users;

public record EmailPayload
{
  public string Address { get; set; } = string.Empty;
  public bool? IsVerified { get; set; }
}
