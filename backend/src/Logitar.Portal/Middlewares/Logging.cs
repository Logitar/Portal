using Logitar.Portal.Application.Logging;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Http.Extensions;

namespace Logitar.Portal.Middlewares;

internal class Logging
{
  private readonly RequestDelegate _next;

  public Logging(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context, ILoggingService loggingService)
  {
    HttpRequest request = context.Request;
    loggingService.Open(context.TraceIdentifier, request.Method, request.GetDisplayUrl(), context.GetClientIpAddress(), context.GetAdditionalInformation());

    try
    {
      await _next(context);
    }
    catch (Exception exception)
    {
      loggingService.Report(exception);

      throw;
    }
    finally
    {
      HttpResponse response = context.Response;
      await loggingService.CloseAndSaveAsync(response.StatusCode);
    }
  }
}
