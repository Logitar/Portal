namespace Logitar.Portal.Domain;

public class InvalidCredentialsException : Exception
{
  public InvalidCredentialsException(string message = "The specified credentials are not valid.", Exception? innerException = null)
    : base(message, innerException)
  {
  }
}
