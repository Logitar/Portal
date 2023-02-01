using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Accounts
{
  public class AccountNotConfirmedException : Exception
  {
    public AccountNotConfirmedException(User user)
      : base($"The user '{user}' does not have a confirmed account.")
    {
      Data["User"] = user.ToString();
    }
  }
}
