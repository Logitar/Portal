using Logitar.Portal.Domain;

namespace Logitar.Portal.Application.Sessions;

public class InvalidRefreshTokenException : InvalidCredentialsException
{
  public InvalidRefreshTokenException(string refreshToken, Exception innerException)
    : base($"The value '{refreshToken}' is not a valid refresh token.", innerException)
  {
    RefreshToken = refreshToken;
  }

  public string RefreshToken
  {
    get => (string)Data[nameof(RefreshToken)]!;
    private set => Data[nameof(RefreshToken)] = value;
  }
}
