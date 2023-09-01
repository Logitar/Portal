using Logitar.Portal.Domain;

namespace Logitar.Portal.Application.ApiKeys;

public class InvalidXApiKeyException : InvalidCredentialsException
{
  public InvalidXApiKeyException(string xApiKey, Exception innerException)
    : base($"The value '{xApiKey}' is not a valid X-API-Key.", innerException)
  {
    XApiKey = xApiKey;
  }

  public string XApiKey
  {
    get => (string)Data[nameof(XApiKey)]!;
    private set => Data[nameof(XApiKey)] = value;
  }
}
