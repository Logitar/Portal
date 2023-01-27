namespace Logitar.Portal.Core.Users
{
  internal class UserCannotDisableItselfException : Exception
  {
    public UserCannotDisableItselfException(User user) : base($"An user '{user}' cannot disable itself.")
    {
      Data["User"] = user.ToString();
    }
  }
}
