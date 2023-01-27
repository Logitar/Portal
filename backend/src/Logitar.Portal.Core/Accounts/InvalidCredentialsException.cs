namespace Logitar.Portal.Core.Accounts
{
  internal class InvalidCredentialsException : Exception
  {
    public InvalidCredentialsException(Exception? innerException = null)
      : base("The specified credentials did not match.", innerException)
    {
    }
  }
}
