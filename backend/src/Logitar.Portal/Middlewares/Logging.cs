using Logitar.Portal.Application.Logging;

namespace Logitar.Portal.Middlewares;

internal class Logging // TODO(fpion): logging
{
  private readonly RequestDelegate _next;

  public Logging(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context, ILoggingService loggingService)
  {
    HttpRequest request = context.Request;
    //loggingService.Start(context.TraceIdentifier, request.Method, request.GetDisplayUrl(), context.GetClientIpAddress(), context.GetAdditionalInformation());

    try
    {
      await _next(context);
    }
    catch (Exception)
    {
      //loggingService.AddException(exception);

      throw;
    }
    finally
    {
      //loggingService.SetActors(_applicationContext.ActorId.ToGuid(), context.GetUser()?.Id, context.GetSessionId());

      //HttpResponse response = context.Response;
      //await loggingService.EndAsync(response.StatusCode);
    }
  }
}
