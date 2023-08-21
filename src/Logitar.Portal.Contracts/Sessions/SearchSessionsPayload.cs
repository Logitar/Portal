namespace Logitar.Portal.Contracts.Sessions;

public record SearchSessionsPayload : SearchPayload
{
  public string? Realm { get; set; }
  public string? UserId { get; set; }

  public bool? IsActive { get; set; }
  public bool? IsPersistent { get; set; }

  public new IEnumerable<SessionSortOption> Sort { get; set; } = Enumerable.Empty<SessionSortOption>();
}
