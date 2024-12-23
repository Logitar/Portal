using Logitar.Identity.Core;

namespace Logitar.Portal.Application.Sessions;

public class InvalidRefreshTokenException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified value is not a valid refresh token.";

  public string RefreshToken
  {
    get => (string)Data[nameof(RefreshToken)]!;
    private set => Data[nameof(RefreshToken)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public InvalidRefreshTokenException(string refreshToken, string? propertyName = null, Exception? innerException = null)
    : base(BuildMessage(refreshToken, propertyName), innerException)
  {
    RefreshToken = refreshToken;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string refreshToken, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RefreshToken), refreshToken)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
