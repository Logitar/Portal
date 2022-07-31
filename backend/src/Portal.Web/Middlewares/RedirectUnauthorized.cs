using Microsoft.AspNetCore.Http.Extensions;

namespace Portal.Web.Middlewares
{
  internal class RedirectUnauthorized
  {
    private readonly RequestDelegate _next;

    public RedirectUnauthorized(RequestDelegate next)
    {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      await _next(context);

      if (context.Response.StatusCode == StatusCodes.Status401Unauthorized && !context.Request.Path.StartsWithSegments("/api"))
      {
        string returnUrl = UriHelper.GetEncodedPathAndQuery(context.Request);

        context.Response.Redirect($"/user/sign-in?returnUrl={returnUrl}");
      }
    }
  }
}
