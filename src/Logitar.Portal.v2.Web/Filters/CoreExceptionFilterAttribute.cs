using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.v2.Core;
using Logitar.Portal.v2.Core.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Portal.v2.Web.Filters;

internal class CoreExceptionFilterAttribute : ExceptionFilterAttribute
{
  private static readonly Dictionary<Type, Action<ExceptionContext>> _handlers = new()
  {
    [typeof(ExternalIdentifierAlreadyUsedException)] = HandleExternalIdentifierAlreadyUsedException,
    [typeof(InvalidCredentialsException)] = HandleInvalidCredentialsException,
    [typeof(InvalidLocaleException)] = HandleInvalidLocaleException,
    [typeof(InvalidTimeZoneEntryException)] = HandleInvalidTimeZoneEntryException,
    [typeof(InvalidUrlException)] = HandleInvalidUrlException,
    [typeof(UniqueNameAlreadyUsedException)] = HandleUniqueNameAlreadyUsedException,
    [typeof(ValidationException)] = HandleValidationException
  };

  public override void OnException(ExceptionContext context)
  {
    if (_handlers.TryGetValue(context.Exception.GetType(), out Action<ExceptionContext>? handler))
    {
      handler(context);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is AggregateNotFoundException exception)
    {
      Dictionary<string, string> value = new()
      {
        ["Code"] = GetCode(exception),
        ["Id"] = exception.Id
      };

      if (exception.ParamName != null)
      {
        value["PropertyName"] = exception.ParamName;
      }

      context.Result = new NotFoundObjectResult(value);
    }
  }

  private static void HandleExternalIdentifierAlreadyUsedException(ExceptionContext context)
  {
    context.Result = new ConflictObjectResult(new { Code = GetCode(context.Exception) });
  }

  private static void HandleInvalidCredentialsException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(new { Code = GetCode(context.Exception) });
  }

  private static void HandleInvalidLocaleException(ExceptionContext context)
  {
    if (context.Exception is InvalidLocaleException exception)
    {
      context.Result = new BadRequestObjectResult(GetPropertyFailure(exception));
    }
  }

  private static void HandleInvalidTimeZoneEntryException(ExceptionContext context)
  {
    if (context.Exception is InvalidTimeZoneEntryException exception)
    {
      context.Result = new BadRequestObjectResult(GetPropertyFailure(exception));
    }
  }

  private static void HandleInvalidUrlException(ExceptionContext context)
  {
    if (context.Exception is InvalidUrlException exception)
    {
      context.Result = new BadRequestObjectResult(GetPropertyFailure(exception));
    }
  }

  private static void HandleUniqueNameAlreadyUsedException(ExceptionContext context)
  {
    if (context.Exception is UniqueNameAlreadyUsedException exception)
    {
      context.Result = new ConflictObjectResult(GetPropertyFailure(exception));
    }
  }

  private static void HandleValidationException(ExceptionContext context)
  {
    if (context.Exception is ValidationException exception)
    {
      context.Result = new BadRequestObjectResult(new { Code = GetCode(exception), exception.Errors });
    }
  }

  private static object GetPropertyFailure(IPropertyFailure error) => new
  {
    Code = GetCode(error),
    PropertyName = error.ParamName,
    AttemptedValue = error.Value
  };

  private static string GetCode(object value) => value.GetType().Name.Remove(nameof(Exception));
}
