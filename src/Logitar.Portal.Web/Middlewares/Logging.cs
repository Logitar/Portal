using Logitar.Portal.Application;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Http.Extensions;

namespace Logitar.Portal.Web.Middlewares;

public class Logging
{
  private readonly IApplicationContext _applicationContext;
  private readonly RequestDelegate _next;

  public Logging(IApplicationContext applicationContext, RequestDelegate next)
  {
    _applicationContext = applicationContext;
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context, ILoggingService loggingService)
  {
    HttpRequest request = context.Request;
    loggingService.Start(context.TraceIdentifier, request.Method, request.GetDisplayUrl(), context.GetClientIpAddress(), context.GetAdditionalInformation());

    try
    {
      await _next(context);
    }
    catch (Exception exception)
    {
      loggingService.AddException(exception);

      throw;
    }
    finally
    {
      loggingService.SetActors(_applicationContext.ActorId.ToGuid(), context.GetUser()?.Id, context.GetSession()?.Id);

      HttpResponse response = context.Response;
      await loggingService.EndAsync(response.StatusCode);
    }
  }
}
