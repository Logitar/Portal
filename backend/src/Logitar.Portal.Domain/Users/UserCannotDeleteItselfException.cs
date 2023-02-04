namespace Logitar.Portal.Domain.Users
{
  public class UserCannotDeleteItselfException : Exception
  {
    public UserCannotDeleteItselfException(User user)
      : base($"The user '{user}' cannot delete itself.")
    {
      Data["User"] = user.ToString();
    }
  }
}
