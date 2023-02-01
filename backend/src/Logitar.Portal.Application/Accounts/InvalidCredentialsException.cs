namespace Logitar.Portal.Application.Accounts
{
  public class InvalidCredentialsException : Exception
  {
    public InvalidCredentialsException(Exception? innerException = null)
      : base("The specified credentials did not match.", innerException)
    {
    }
  }
}
