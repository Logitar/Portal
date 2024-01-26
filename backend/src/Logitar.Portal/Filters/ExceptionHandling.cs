using FluentValidation;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Errors;
using Logitar.Portal.Contracts.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Portal.Filters;

internal class ExceptionHandling : ExceptionFilterAttribute
{
  private static readonly Dictionary<Type, Func<ExceptionContext, IActionResult>> _handlers = new()
  {
    [typeof(ConfigurationAlreadyInitializedException)] = HandleConfigurationAlreadyInitializedException,
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

  private static ConflictObjectResult HandleConfigurationAlreadyInitializedException(ExceptionContext context)
  {
    ConfigurationAlreadyInitializedException exception = (ConfigurationAlreadyInitializedException)context.Exception;
    return new ConflictObjectResult(new Error(exception.GetErrorCode(), exception.Message));
  }

  private static BadRequestObjectResult HandleValidationException(ExceptionContext context)
  {
    ValidationException exception = (ValidationException)context.Exception;
    return new BadRequestObjectResult(new ValidationError(exception));
  }
}
