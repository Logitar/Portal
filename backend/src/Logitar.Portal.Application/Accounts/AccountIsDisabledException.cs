using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Accounts
{
  public class AccountIsDisabledException : Exception
  {
    public AccountIsDisabledException(User user)
      : base($"The user '{user}' is disabled.")
    {
      Data["User"] = user.ToString();
    }
  }
}
