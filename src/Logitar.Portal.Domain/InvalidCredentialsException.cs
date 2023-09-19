namespace Logitar.Portal.Domain;

public class InvalidCredentialsException : Exception
{
  public const string ErrorMessage = "The specified credentials are not valid.";

  public InvalidCredentialsException(string message = ErrorMessage, Exception? innerException = null)
    : base(message, innerException)
  {
  }
}
