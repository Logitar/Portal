namespace Logitar.Portal.Core.Users
{
  internal class UserAlreadyDisabledException : Exception
  {
    public UserAlreadyDisabledException(User user) : base($"The user '{user}' is already disabled.")
    {
      Data["User"] = user.ToString();
    }
  }
}
