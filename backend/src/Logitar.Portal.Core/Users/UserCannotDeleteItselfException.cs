namespace Logitar.Portal.Core.Users
{
  internal class UserCannotDeleteItselfException : Exception
  {
    public UserCannotDeleteItselfException(User user) : base($"An user '{user}' cannot delete itself.")
    {
      Data["User"] = user.ToString();
    }
  }
}
