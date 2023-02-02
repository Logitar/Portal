namespace Logitar.Portal.Domain.Users
{
  public class UserAlreadyDisabledException : Exception
  {
    public UserAlreadyDisabledException(User user)
      : base($"The user '{user}' is already disabled.")
    {
      Data["User"] = user.ToString();
    }
  }
}
