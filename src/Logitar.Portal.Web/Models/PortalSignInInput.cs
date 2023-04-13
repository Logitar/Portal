namespace Logitar.Portal.Web.Models;

public record PortalSignInInput
{
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public bool Remember { get; set; }
}
