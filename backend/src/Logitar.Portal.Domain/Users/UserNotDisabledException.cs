namespace Logitar.Portal.Domain.Users
{
  public class UserNotDisabledException : Exception
  {
    public UserNotDisabledException(User user)
      : base($"The user '{user}' is not disabled.")
    {
      Data["User"] = user.ToString();
    }
  }
}
