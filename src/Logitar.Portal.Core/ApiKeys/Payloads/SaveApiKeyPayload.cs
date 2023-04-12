namespace Logitar.Portal.Core.ApiKeys.Payloads
{
  public class SaveApiKeyPayload
  {
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
  }
}
