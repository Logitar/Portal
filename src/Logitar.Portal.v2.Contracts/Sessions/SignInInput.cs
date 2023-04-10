namespace Logitar.Portal.v2.Contracts.Sessions;

public record SignInInput
{
  public string Realm { get; set; } = string.Empty;

  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public bool Remember { get; set; }

  public IEnumerable<CustomAttribute>? CustomAttributes { get; set; }
}
