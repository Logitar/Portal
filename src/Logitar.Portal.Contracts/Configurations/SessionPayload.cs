namespace Logitar.Portal.Contracts.Configurations;

public record SessionPayload
{
  public bool IsPersistent { get; set; }

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();
}
