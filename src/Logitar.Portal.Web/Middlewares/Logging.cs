using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Logging;
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
    await loggingService.StartAsync(correlationId: context.TraceIdentifier, request.Method,
      destination: request.GetDisplayUrl(), source: context.GetClientIpAddress(),
      context.GetAdditionalInformation());

    try
    {
      await _next(context);
    }
    catch (Exception exception)
    {
      await loggingService.AddErrorAsync(Error.From(exception), activityId: null);

      throw;
    }
    finally
    {
      await loggingService.SetActorsAsync(_applicationContext.ActorId.ToGuid(), context.GetUser()?.Id, context.GetSession()?.Id);

      HttpResponse response = context.Response;
      await loggingService.EndAsync(response.StatusCode);
    }
  }
}
