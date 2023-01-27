using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Web.Models.Users;

namespace Logitar.Portal.Web.Extensions
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
