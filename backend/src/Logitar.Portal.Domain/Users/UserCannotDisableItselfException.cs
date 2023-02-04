namespace Logitar.Portal.Domain.Users
{
  public class UserCannotDisableItselfException : Exception
  {
    public UserCannotDisableItselfException(User user)
      : base($"The user '{user}' cannot disable itself.")
    {
      Data["User"] = user.ToString();
    }
  }
}
