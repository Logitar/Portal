namespace Logitar.Portal.Core2.Accounts
{
  internal class AccountIsDisabledException : Exception
  {
    public AccountIsDisabledException(AggregateId userId) : base($"The user 'Id={userId}' is disabled.")
    {
    }
  }
}
