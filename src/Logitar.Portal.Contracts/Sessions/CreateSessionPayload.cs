namespace Logitar.Portal.Contracts.Sessions;

public record CreateSessionPayload
{
  public string UserId { get; set; } = string.Empty;

  public bool IsPersistent { get; set; }

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();
}
