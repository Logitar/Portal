namespace Logitar.Portal.Domain.ApiKeys;

public class ApiKeyIsExpiredException : Exception
{
  public ApiKeyIsExpiredException(ApiKeyAggregate apiKey) : base($"The API key '{apiKey}' is expired.")
  {
    ApiKey = apiKey.ToString();
  }

  public string ApiKey
  {
    get => (string)Data[nameof(ApiKey)]!;
    private set => Data[nameof(ApiKey)] = value;
  }
}
