using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Accounts
{
  public class UserHasNoPasswordException : Exception
  {
    public UserHasNoPasswordException(User user)
      : base($"The user '{user}' has no password.")
    {
      Data["User"] = user.ToString();
    }
  }
}
