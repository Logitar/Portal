namespace Logitar.Portal.Contracts.ApiKeys
{
  public record UpdateApiKeyPayload
  {
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
  }
}
