using System.Net;

namespace Logitar.Portal.Core.Users
{
  internal class UserCannotDeleteItselfException : ApiException
  {
    public UserCannotDeleteItselfException(User user)
      : base(HttpStatusCode.BadRequest, $"An user '{user}' cannot delete itself.")
    {
      User = user ?? throw new ArgumentNullException(nameof(user));
      Value = new { code = nameof(UserCannotDeleteItselfException).Remove(nameof(Exception)) };
    }

    public User User { get; }
  }
}
