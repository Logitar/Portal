namespace Logitar.Portal.Contracts.Sessions;

public record SignInPayload
{
  public string? Realm { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public bool IsPersistent { get; set; }

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();
}
