namespace Logitar.Portal.Web.Models.Account;

public record PortalSignInPayload
{
  public string UniqueName { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public bool Remember { get; set; }
}
