namespace Logitar.Portal.v2.Core.Users;

public class InvalidCredentialsException : Exception
{
  public InvalidCredentialsException(string message, Exception? innerException = null)
    : base(message, innerException)
  {
  }
}
