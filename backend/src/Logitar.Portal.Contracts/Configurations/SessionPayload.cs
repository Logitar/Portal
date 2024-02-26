namespace Logitar.Portal.Contracts.Configurations;

public record SessionPayload
{
  public List<CustomAttribute> CustomAttributes { get; set; }

  public SessionPayload()
  {
    CustomAttributes = [];
  }

  public SessionPayload(IEnumerable<CustomAttribute> customAttributes) : this()
  {
    CustomAttributes.AddRange(customAttributes);
  }
}
