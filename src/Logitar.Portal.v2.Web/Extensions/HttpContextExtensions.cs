using Logitar.Portal.v2.Web.Models;

namespace Logitar.Portal.v2.Web.Extensions;

internal static class HttpContextExtensions
{
  /// <summary>
  /// TODO(fpion): implement
  /// </summary>
  /// <param name="context"></param>
  /// <returns></returns>
  public static CurrentUser GetCurrentUser(this HttpContext _) => new();
}
