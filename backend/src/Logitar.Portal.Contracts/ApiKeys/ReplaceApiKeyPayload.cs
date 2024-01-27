namespace Logitar.Portal.Contracts.ApiKeys;

public record ReplaceApiKeyPayload
{
  public string DisplayName { get; set; }
  public string? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public List<string> Roles { get; set; }

  public ReplaceApiKeyPayload() : this(string.Empty)
  {
  }

  public ReplaceApiKeyPayload(string displayName)
  {
    DisplayName = displayName;
    CustomAttributes = [];
    Roles = [];
  }
}
