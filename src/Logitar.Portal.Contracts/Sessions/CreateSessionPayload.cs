namespace Logitar.Portal.Contracts.Sessions;

public record CreateSessionPayload
{
  public Guid UserId { get; set; }

  public bool IsPersistent { get; set; }

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();
}
