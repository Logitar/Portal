using Logitar.Portal.Core.Users;
using System.Net;

namespace Logitar.Portal.Core.Accounts
{
  internal class UserHasNoPasswordException : ApiException
  {
    public UserHasNoPasswordException(User user)
      : base(HttpStatusCode.BadRequest, $"The user '{user}' has no password.")
    {
      User = user ?? throw new ArgumentNullException(nameof(user));
      Value = new { code = nameof(UserHasNoPasswordException).Remove(nameof(Exception)) };
    }

    public User User { get; }
  }
}
