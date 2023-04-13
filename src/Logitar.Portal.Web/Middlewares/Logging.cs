using Logitar.Portal.Core.Logging;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Http.Extensions;

namespace Logitar.Portal.Web.Middlewares;

public class Logging
{
  private readonly RequestDelegate _next;

  public Logging(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context, ILoggingService loggingService)
  {
    HttpRequest request = context.Request;
    await loggingService.StartAsync(correlationId: context.TraceIdentifier, request.Method,
      destination: request.GetDisplayUrl(), source: context.GetClientIpAddress(),
      context.GetAdditionalInformation());

    await _next(context);

    HttpResponse response = context.Response;
    await loggingService.EndAsync(response.StatusCode);
  }
}
