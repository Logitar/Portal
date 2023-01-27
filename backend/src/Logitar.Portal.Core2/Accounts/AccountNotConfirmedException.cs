namespace Logitar.Portal.Core2.Accounts
{
  internal class AccountNotConfirmedException : Exception
  {
    public AccountNotConfirmedException(AggregateId userId) : base($"The user 'Id={userId}' does not have a confirmed account.")
    {
    }
  }
}
