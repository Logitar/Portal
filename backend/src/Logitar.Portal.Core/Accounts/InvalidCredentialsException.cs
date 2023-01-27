namespace Logitar.Portal.Core.Accounts
{
  internal class InvalidCredentialsException : Exception
  {
    public InvalidCredentialsException() : base("The specified credentials did not match.")
    {
    }
  }
}
