using Logitar.Portal.Core;
using Logitar.Portal.Domain.Users;
using System.Net;

namespace Logitar.Portal.Application.Users
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
