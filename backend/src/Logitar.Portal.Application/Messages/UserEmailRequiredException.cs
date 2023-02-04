using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Messages
{
  public class UserEmailRequiredException : Exception
  {
    public UserEmailRequiredException(User user) : base($"The user '{user}' email is required.")
    {
      Data["User"] = user.ToString();
    }
  }
}
