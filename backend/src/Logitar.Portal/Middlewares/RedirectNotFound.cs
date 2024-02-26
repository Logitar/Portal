using Microsoft.AspNetCore.Http.Extensions;

namespace Logitar.Portal.Middlewares;

internal class RedirectNotFound
{
  private readonly RequestDelegate _next;

  public RedirectNotFound(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    await _next(context);

    if (context.Response.StatusCode == StatusCodes.Status404NotFound && !context.Request.Path.StartsWithSegments("/api"))
    {
      context.Response.Redirect($"/app/{UriHelper.GetEncodedPathAndQuery(context.Request).Trim('/')}");
    }
  }
}
