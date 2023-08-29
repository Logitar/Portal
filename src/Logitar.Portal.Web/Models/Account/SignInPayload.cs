namespace Logitar.Portal.Web.Models.Account;

public record SignInPayload
{
  public string UniqueName { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public bool Remember { get; set; }
}
