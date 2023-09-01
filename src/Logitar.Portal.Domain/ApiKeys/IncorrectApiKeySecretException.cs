namespace Logitar.Portal.Domain.ApiKeys;

public class IncorrectApiKeySecretException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified secret did not match the API key.";

  public IncorrectApiKeySecretException(ApiKeyAggregate apiKey, string secret)
    : base(BuildMessage(apiKey, secret))
  {
    ApiKey = apiKey.ToString();
    Secret = secret;
  }

  public string ApiKey
  {
    get => (string)Data[nameof(ApiKey)]!;
    private set => Data[nameof(ApiKey)] = value;
  }
  public string Secret
  {
    get => (string)Data[nameof(Secret)]!;
    private set => Data[nameof(Secret)] = value;
  }

  private static string BuildMessage(ApiKeyAggregate apiKey, string secret)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("ApiKey: ").Append(apiKey).AppendLine();
    message.Append("Secret: ").AppendLine(secret);

    return message.ToString();
  }
}
