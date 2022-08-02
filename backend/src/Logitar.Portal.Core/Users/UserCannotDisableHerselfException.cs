using System.Net;

namespace Logitar.Portal.Core.Users
{
  internal class UserCannotDisableHerselfException : ApiException
  {
    public UserCannotDisableHerselfException(User user)
      : base(HttpStatusCode.BadRequest, $"The user '{user}' cannot disable herself.")
    {
      User = user ?? throw new ArgumentNullException(nameof(user));
      Value = new { code = nameof(UserCannotDisableHerselfException).Remove(nameof(Exception)) };
    }

    public User User { get; }
  }
}
