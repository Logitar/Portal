using FluentValidation;
using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Realms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Portal.Filters;

internal class ExceptionHandlingAttribute : ExceptionFilterAttribute
{
  private readonly Dictionary<Type, Func<ExceptionContext, IActionResult>> _handlers = new()
  {
    [typeof(ConfigurationAlreadyInitializedException)] = HandleConfigurationAlreadyInitializedException,
    [typeof(EmailAddressAlreadyUsedException)] = HandleEmailAddressAlreadyUsedException,
    [typeof(InvalidAggregateIdException)] = HandleInvalidAggregateIdException,
    [typeof(InvalidGenderException)] = HandleInvalidGenderException,
    [typeof(InvalidLocaleException)] = HandleInvalidLocaleException,
    [typeof(InvalidTimeZoneEntryException)] = HandleInvalidTimeZoneEntryException,
    [typeof(InvalidUrlException)] = HandleInvalidUrlException,
    [typeof(NotImplementedException)] = HandleNotImplementedException,
    [typeof(SessionIsNotActiveException)] = HandleSessionIsNotActiveException,
    [typeof(UniqueSlugAlreadyUsedException)] = HandleUniqueSlugAlreadyUsedException,
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
    else if (context.Exception is InvalidCredentialsException)
    {
      context.Result = new BadRequestObjectResult(new { ErrorCode = "InvalidCredentials" });
      context.ExceptionHandled = true;
    }
    else if (context.Exception is TooManyResultsException)
    {
      context.Result = new BadRequestObjectResult(new { ErrorCode = "TooManyResults" });
      context.ExceptionHandled = true;
    }
    else if (context.Exception is UniqueNameAlreadyUsedException uniqueNameAlreadyUsed)
    {
      context.Result = new ConflictObjectResult(uniqueNameAlreadyUsed.Failure);
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }
  }

  private static IActionResult HandleConfigurationAlreadyInitializedException(ExceptionContext context)
  {
    return new ConflictObjectResult(new { ErrorCode = "ConfigurationAlreadyInitialized" });
  }

  private static IActionResult HandleEmailAddressAlreadyUsedException(ExceptionContext context)
  {
    return new ConflictObjectResult(((EmailAddressAlreadyUsedException)context.Exception).Failure);
  }

  private static IActionResult HandleInvalidAggregateIdException(ExceptionContext context)
  {
    return new BadRequestObjectResult(((InvalidAggregateIdException)context.Exception).Failure);
  }

  private static IActionResult HandleInvalidGenderException(ExceptionContext context)
  {
    return new BadRequestObjectResult(((InvalidGenderException)context.Exception).Failure);
  }

  private static IActionResult HandleInvalidLocaleException(ExceptionContext context)
  {
    return new BadRequestObjectResult(((InvalidLocaleException)context.Exception).Failure);
  }

  private static IActionResult HandleInvalidTimeZoneEntryException(ExceptionContext context)
  {
    return new BadRequestObjectResult(((InvalidTimeZoneEntryException)context.Exception).Failure);
  }

  private static IActionResult HandleInvalidUrlException(ExceptionContext context)
  {
    return new BadRequestObjectResult(((InvalidUrlException)context.Exception).Failure);
  }

  private static IActionResult HandleNotImplementedException(ExceptionContext context)
  {
    return new JsonResult(new { ErrorCode = "NotImplemented" })
    {
      StatusCode = StatusCodes.Status501NotImplemented
    };
  }

  private static IActionResult HandleSessionIsNotActiveException(ExceptionContext _)
  {
    return new BadRequestObjectResult(new { ErrorCode = "SessionIsNotActive" });
  }

  private static IActionResult HandleUniqueSlugAlreadyUsedException(ExceptionContext context)
  {
    return new ConflictObjectResult(((UniqueSlugAlreadyUsedException)context.Exception).Failure);
  }

  private static IActionResult HandleUserIsDisabledException(ExceptionContext _)
  {
    return new BadRequestObjectResult(new { ErrorCode = "UserIsDisabled" });
  }

  private static IActionResult HandleUserIsNotConfirmedException(ExceptionContext _)
  {
    return new BadRequestObjectResult(new { ErrorCode = "UserIsNotConfirmed" });
  }

  private static IActionResult HandleValidationException(ExceptionContext context)
  {
    return new BadRequestObjectResult(new
    {
      ErrorCode = "Validation",
      ((ValidationException)context.Exception).Errors
    });
  }
}
