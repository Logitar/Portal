namespace Logitar.Portal.Core.Users
{
  internal class UserNotDisabledException : Exception
  {
    public UserNotDisabledException(User user) : base($"The user '{user}' is not disabled.")
    {
      Data["User"] = user.ToString();
    }
  }
}
