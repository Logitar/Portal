namespace Logitar.Portal.Core.Accounts
{
  internal class AccountNotConfirmedException : Exception
  {
    public AccountNotConfirmedException(AggregateId userId) : base($"The user 'Id={userId}' does not have a confirmed account.")
    {
    }
  }
}
