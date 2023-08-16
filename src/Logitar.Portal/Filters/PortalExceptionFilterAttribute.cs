using FluentValidation;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Portal.Filters;

internal class PortalExceptionFilterAttribute : ExceptionFilterAttribute
{
  private readonly Dictionary<Type, Action<ExceptionContext>> _handlers = new()
  {
    [typeof(ConfigurationAlreadyInitializedException)] = HandleConfigurationAlreadyInitializedException,
    [typeof(InvalidLocaleException)] = HandleInvalidLocaleException,
    [typeof(ValidationException)] = HandleValidationException
  };

  public override void OnException(ExceptionContext context)
  {
    if (_handlers.TryGetValue(context.Exception.GetType(), out Action<ExceptionContext>? handler))
    {
      handler(context);
      context.ExceptionHandled = true;
    }
  }

  private static void HandleConfigurationAlreadyInitializedException(ExceptionContext context)
  {
    context.Result = new JsonResult(new { Code = "ConfigurationAlreadyInitialized" })
    {
      StatusCode = StatusCodes.Status403Forbidden
    };
  }

  private static void HandleInvalidLocaleException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(((InvalidLocaleException)context.Exception).Failure);
  }

  private static void HandleValidationException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(new { ((ValidationException)context.Exception).Errors });
  }
}
