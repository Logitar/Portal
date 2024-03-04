using Logitar.Portal.Application.Logging;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Portal.Web.Filters;

internal class LoggingFilter : ActionFilterAttribute
{
  private readonly ILoggingService _loggingService;

  public LoggingFilter(ILoggingService loggingService)
  {
    _loggingService = loggingService;
  }

  public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
  {
    if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
    {
      Operation operation = new(descriptor.ControllerName, descriptor.ActionName);
      _loggingService.SetOperation(operation);
    }

    await base.OnActionExecutionAsync(context, next);
  }
}
