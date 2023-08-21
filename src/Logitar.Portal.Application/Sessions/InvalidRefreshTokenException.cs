using Logitar.Identity.Domain;

namespace Logitar.Portal.Application.Sessions;

public class InvalidRefreshTokenException : InvalidCredentialsException
{
  public InvalidRefreshTokenException(string refreshToken, Exception? innerException = null)
    : base($"The refresh token '{refreshToken}' is not valid.", innerException)
  {
    RefreshToken = refreshToken;
  }

  public string RefreshToken
  {
    get => (string)Data[nameof(RefreshToken)]!;
    private set => Data[nameof(RefreshToken)] = value;
  }
}
