namespace Logitar.Portal.Core.Users;

public class AccountIsDisabledException : Exception
{
  public AccountIsDisabledException(UserAggregate user)
    : base($"The user account '{user}' is disabled.")
  {
    Data["User"] = user.ToString();
  }
}
