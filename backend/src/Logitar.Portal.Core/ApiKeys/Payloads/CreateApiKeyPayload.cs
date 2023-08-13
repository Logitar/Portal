namespace Logitar.Portal.Core.ApiKeys.Payloads
{
  public class CreateApiKeyPayload : SaveApiKeyPayload
  {
    public DateTime? ExpiresAt { get; set; }
  }
}
