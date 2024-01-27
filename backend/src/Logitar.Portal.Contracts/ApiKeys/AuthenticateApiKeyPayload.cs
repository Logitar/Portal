namespace Logitar.Portal.Contracts.ApiKeys;

public record AuthenticateApiKeyPayload
{
  public string ApiKey { get; set; }

  public AuthenticateApiKeyPayload() : this(string.Empty)
  {
  }

  public AuthenticateApiKeyPayload(string apiKey)
  {
    ApiKey = apiKey;
  }
}
