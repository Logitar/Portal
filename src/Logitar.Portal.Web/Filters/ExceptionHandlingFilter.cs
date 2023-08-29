using FluentValidation;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Sessions;
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
    [typeof(SessionIsNotActiveException)] = HandleSessionIsNotActiveException,
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
    else if (context.Exception is AggregateNotFoundException aggregateNotFound)
    {
      context.Result = new NotFoundObjectResult(aggregateNotFound.Failure);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is InvalidCredentialsException invalidCredentials)
    {
      context.Result = new BadRequestObjectResult(new ErrorInfo(invalidCredentials, "The specified credentials are not valid."));
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }
  }

  private static IActionResult HandleConfigurationAlreadyInitializedException(ExceptionContext context)
  {
    return new JsonResult(new ErrorInfo(context.Exception, "The configuration has already been initialized."))
    {
      StatusCode = StatusCodes.Status403Forbidden
    };
  }

  private static IActionResult HandleInvalidLocaleException(ExceptionContext context)
  {
    return new BadRequestObjectResult(((InvalidLocaleException)context.Exception).Failure);
  }

  public static IActionResult HandleSessionIsNotActiveException(ExceptionContext context)
  {
    return new BadRequestObjectResult(new ErrorInfo(context.Exception, "The session is not active."));
  }

  private static IActionResult HandleUserIsDisabledException(ExceptionContext context)
  {
    return new BadRequestObjectResult(new ErrorInfo(context.Exception, "The user is disabled."));
  }

  private static IActionResult HandleUserIsNotConfirmedException(ExceptionContext context)
  {
    return new BadRequestObjectResult(new ErrorInfo(context.Exception, "The user is not confirmed."));
  }

  private static IActionResult HandleValidationException(ExceptionContext context)
  {
    return new BadRequestObjectResult(new { ((ValidationException)context.Exception).Errors });
  }
}
