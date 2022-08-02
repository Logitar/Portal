using System.Net;

namespace Logitar.Portal.Core.Users
{
  internal class UserCannotDeleteHerselfException : ApiException
  {
    public UserCannotDeleteHerselfException(User user)
      : base(HttpStatusCode.BadRequest, $"An user '{user}' cannot delete herself.")
    {
      User = user ?? throw new ArgumentNullException(nameof(user));
      Value = new { code = nameof(UserCannotDeleteHerselfException).Remove(nameof(Exception)) };
    }

    public User User { get; }
  }
}
