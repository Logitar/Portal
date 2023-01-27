using Logitar.Portal.Core2.Sessions.Models;
using Logitar.Portal.Web2.Models.Users;

namespace Logitar.Portal.Web2.Extensions
{
  internal static class HttpContextExtensions
  {
    public static CurrentUser GetCurrentUser(this HttpContext context) => new();

    public static void SetSession(this HttpContext context, SessionModel session)
    {
      // TODO(fpion): implement
    }
  }
}
