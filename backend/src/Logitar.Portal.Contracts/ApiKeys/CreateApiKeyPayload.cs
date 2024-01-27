namespace Logitar.Portal.Contracts.ApiKeys;

public record CreateApiKeyPayload
{
  public string? Id { get; set; }

  public string DisplayName { get; set; }
  public string? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public List<string> Roles { get; set; }

  public CreateApiKeyPayload() : this(string.Empty)
  {
  }

  public CreateApiKeyPayload(string displayName)
  {
    DisplayName = displayName;
    CustomAttributes = [];
    Roles = [];
  }
}
