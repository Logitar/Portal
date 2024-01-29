namespace Logitar.Portal.Contracts.Configurations;

public record SessionPayload
{
  public List<CustomAttribute> CustomAttributes { get; set; }

  public SessionPayload() : this([])
  {
  }

  public SessionPayload(IEnumerable<CustomAttribute> customAttributes)
  {
    CustomAttributes = customAttributes.ToList();
  }
}
