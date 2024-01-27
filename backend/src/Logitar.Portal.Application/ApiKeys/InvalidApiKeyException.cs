using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Application.ApiKeys;

public class InvalidApiKeyException : InvalidCredentialsException
{
  public new const string ErrorMessage = "The specified API key is not valid.";

  public string ApiKey
  {
    get => (string)Data[nameof(ApiKey)]!;
    private set => Data[nameof(ApiKey)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public InvalidApiKeyException(string refreshToken, string? propertyName = null, Exception? innerException = null)
    : base(BuildMessage(refreshToken, propertyName), innerException)
  {
    ApiKey = refreshToken;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string refreshToken, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ApiKey), refreshToken)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
