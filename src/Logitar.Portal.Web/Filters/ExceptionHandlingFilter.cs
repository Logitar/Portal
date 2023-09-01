using FluentValidation;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Portal.Web.Filters;

internal class ExceptionHandlingFilter : ExceptionFilterAttribute
{
  private readonly Dictionary<Type, Func<ExceptionContext, IActionResult>> _handlers = new()
  {
    [typeof(ApiKeyIsExpiredException)] = HandleApiKeyIsExpiredException,
    [typeof(CannotPostponeExpirationException)] = HandleCannotPostponeExpirationException,
    [typeof(ConfigurationAlreadyInitializedException)] = HandleConfigurationAlreadyInitializedException,
    [typeof(EmailAddressAlreadyUsedException)] = HandleEmailAddressAlreadyUsedException,
    [typeof(InvalidGenderException)] = HandleInvalidGenderException,
    [typeof(InvalidLocaleException)] = HandleInvalidLocaleException,
    [typeof(InvalidTimeZoneEntryException)] = HandleInvalidTimeZoneEntryException,
    [typeof(InvalidUrlException)] = HandleInvalidUrlException,
    [typeof(RolesNotFoundException)] = HandleRolesNotFoundException,
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
    else if (context.Exception is IdentifierAlreadyUsedException identifierAlreadyUsed)
    {
      context.Result = new ConflictObjectResult(identifierAlreadyUsed.Failure);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is InvalidCredentialsException invalidCredentials)
    {
      context.Result = new BadRequestObjectResult(new ErrorInfo(invalidCredentials, "The specified credentials are not valid."));
      context.ExceptionHandled = true;
    }
    else if (context.Exception is TooManyResultsException tooManyResults)
    {
      context.Result = new BadRequestObjectResult(new ErrorInfo(tooManyResults, "There are too many results."));
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

  private static IActionResult HandleApiKeyIsExpiredException(ExceptionContext context)
  {
    return new BadRequestObjectResult(new ErrorInfo(context.Exception, "The API key is expired."));
  }

  private static IActionResult HandleCannotPostponeExpirationException(ExceptionContext context)
  {
    return new BadRequestObjectResult(((CannotPostponeExpirationException)context.Exception).Failure);
  }

  private static IActionResult HandleConfigurationAlreadyInitializedException(ExceptionContext context)
  {
    return new JsonResult(new ErrorInfo(context.Exception, "The configuration has already been initialized."))
    {
      StatusCode = StatusCodes.Status403Forbidden
    };
  }

  private static IActionResult HandleEmailAddressAlreadyUsedException(ExceptionContext context)
  {
    return new ConflictObjectResult(((EmailAddressAlreadyUsedException)context.Exception).Failure);
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

  private static IActionResult HandleRolesNotFoundException(ExceptionContext context)
  {
    return new NotFoundObjectResult(((RolesNotFoundException)context.Exception).Failure);
  }

  private static IActionResult HandleSessionIsNotActiveException(ExceptionContext context)
  {
    return new BadRequestObjectResult(new ErrorInfo(context.Exception, "The session is not active."));
  }

  private static IActionResult HandleUniqueSlugAlreadyUsedException(ExceptionContext context)
  {
    return new ConflictObjectResult(((UniqueSlugAlreadyUsedException)context.Exception).Failure);
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
