namespace Logitar.Portal.Contracts.ApiKeys
{
  public record CreateApiKeyPayload
  {
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime? ExpiresOn { get; set; }
  }
}
