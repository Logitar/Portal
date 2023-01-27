namespace Logitar.Portal.Core2.Accounts
{
  internal class InvalidCredentialsException : Exception
  {
    public InvalidCredentialsException() : base("The specified credentials did not match.")
    {
    }
  }
}
