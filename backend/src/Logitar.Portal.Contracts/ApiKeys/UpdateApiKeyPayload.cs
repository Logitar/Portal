namespace Logitar.Portal.Contracts.ApiKeys
{
  public record UpdateApiKeyPayload
  {
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
  }
}
