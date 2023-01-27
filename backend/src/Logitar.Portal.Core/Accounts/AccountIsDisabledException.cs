namespace Logitar.Portal.Core.Accounts
{
  internal class AccountIsDisabledException : Exception
  {
    public AccountIsDisabledException(AggregateId userId) : base($"The user 'Id={userId}' is disabled.")
    {
    }
  }
}
