using System.Net;

namespace Logitar.Portal.Core.Users
{
  internal class UserCannotDisableItselfException : ApiException
  {
    public UserCannotDisableItselfException(User user)
      : base(HttpStatusCode.BadRequest, $"The user '{user}' cannot disable itself.")
    {
      User = user ?? throw new ArgumentNullException(nameof(user));
      Value = new { code = nameof(UserCannotDisableItselfException).Remove(nameof(Exception)) };
    }

    public User User { get; }
  }
}
