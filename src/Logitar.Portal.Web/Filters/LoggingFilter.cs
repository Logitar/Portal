using Logitar.Portal.Core.Logging;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Portal.Web.Filters;

public class LoggingFilter : ActionFilterAttribute
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
      await _loggingService.SetOperationAsync(type: descriptor.ControllerName, name: descriptor.ActionName);
    }

    await base.OnActionExecutionAsync(context, next);
  }
}
