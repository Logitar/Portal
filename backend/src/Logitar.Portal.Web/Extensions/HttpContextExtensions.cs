using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Web.Models.Users;
using System.Text;

namespace Logitar.Portal.Web.Extensions
{
  internal static class HttpContextExtensions
  {
    private const string SessionIdKey = "SessionId";

    public static CurrentUser GetCurrentUser(this HttpContext context) => new();

    public static void SetSession(this HttpContext context, SessionModel session)
    {
      context.Session.Set(SessionIdKey, Encoding.ASCII.GetBytes(session.Id));

      if (session.RenewToken != null)
      {
        context.Response.Cookies.Append(Constants.Cookies.RenewToken, session.RenewToken, Constants.Cookies.RenewTokenOptions);
      }
    }
  }
}
