using FluentValidation;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Portal.Web.Filters;

internal class ExceptionHandlingFilter : ExceptionFilterAttribute
{
  private readonly Dictionary<Type, Func<ExceptionContext, IActionResult>> _handlers = new()
  {
    [typeof(ConfigurationAlreadyInitializedException)] = HandleConfigurationAlreadyInitializedException,
    [typeof(InvalidLocaleException)] = HandleInvalidLocaleException,
    [typeof(UserIsDisabledException)] = HandleUserIsDisabledException,
    [typeof(UserIsNotConfirmedException)] = HandleUserIsNotConfirmedException,
    [typeof(ValidationException)] = HandleValidationException
  };

  public override void OnException(ExceptionContext context)
  {
    if (_handlers.TryGetValue(context.Exception.GetType(), out Func<ExceptionContext, IActionResult>? handler))
    {
      context.Result = handler(context);
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }
  }

  private static IActionResult HandleConfigurationAlreadyInitializedException(ExceptionContext _)
  {
    return new JsonResult(new { ErrorCode = "ConfigurationAlreadyInitialized" })
    {
      StatusCode = StatusCodes.Status403Forbidden
    };
  }

  private static IActionResult HandleInvalidLocaleException(ExceptionContext context)
  {
    return new BadRequestObjectResult(((InvalidLocaleException)context.Exception).Failure);
  }

  private static IActionResult HandleUserIsDisabledException(ExceptionContext _)
  {
    return new BadRequestObjectResult(new { ErrorCode = "UserIsDisabled" });
  }

  private static IActionResult HandleUserIsNotConfirmedException(ExceptionContext _)
  {
    return new BadRequestObjectResult(new { ErrorCode = "UserIsDisabled" });
  }

  private static IActionResult HandleValidationException(ExceptionContext context)
  {
    return new BadRequestObjectResult(new { ((ValidationException)context.Exception).Errors });
  }
}
