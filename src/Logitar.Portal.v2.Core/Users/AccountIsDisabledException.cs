namespace Logitar.Portal.v2.Core.Users;

public class AccountIsDisabledException : Exception
{
  public AccountIsDisabledException(UserAggregate user)
    : base($"The user account '{user}' is disabled.")
  {
    Data["User"] = user.ToString();
  }
}
