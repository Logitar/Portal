using System.Net;

namespace Portal.Core.Users
{
  internal class UserAlreadyDisabledException : ApiException
  {
    public UserAlreadyDisabledException(User user)
      : base(HttpStatusCode.BadRequest, $"The user '{user}' is already disabled.")
    {
      User = user ?? throw new ArgumentNullException(nameof(user));
      Value = new { code = nameof(UserAlreadyDisabledException).Remove(nameof(Exception)) };
    }

    public User User { get; }
  }
}
