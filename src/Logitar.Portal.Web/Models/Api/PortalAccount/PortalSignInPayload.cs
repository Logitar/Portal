namespace Logitar.Portal.Web.Models.Api.PortalAccount
{
  public class PortalSignInPayload
  {
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool Remember { get; set; }
  }
}
