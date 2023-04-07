namespace Logitar.Portal.v2.Core.Users;

public class InvalidCredentialsException : Exception
{
  public InvalidCredentialsException(string message) : base(message)
  {
  }
}
