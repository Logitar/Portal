namespace Logitar.Portal.Core.Users
{
  internal class UserHasNoPasswordException : Exception
  {
    public UserHasNoPasswordException(User user) : base($"The user '{user}' has no password.")
    {
      Data["User"] = user.ToString();
    }
  }
}
