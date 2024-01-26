namespace Logitar.Portal.Contracts.Configurations;

public record SessionPayload
{
  public List<CustomAttribute> CustomAttributes { get; set; } = [];
}
