using Logitar.Portal.Core;
using Logitar.Portal.Core.Logging;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Http.Extensions;

namespace Logitar.Portal.Web.Middlewares;

public class Logging
{
  private readonly ICurrentActor _currentActor;
  private readonly RequestDelegate _next;

  public Logging(ICurrentActor currentActor, RequestDelegate next)
  {
    _currentActor = currentActor;
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context, ILoggingService loggingService)
  {
    HttpRequest request = context.Request;
    await loggingService.StartAsync(correlationId: context.TraceIdentifier, request.Method,
      destination: request.GetDisplayUrl(), source: context.GetClientIpAddress(),
      context.GetAdditionalInformation());

    await _next(context);

    await loggingService.SetActorsAsync(_currentActor.Id.ToGuid(), context.GetUser()?.Id, context.GetSession()?.Id);

    HttpResponse response = context.Response;
    await loggingService.EndAsync(response.StatusCode);
  }
}
