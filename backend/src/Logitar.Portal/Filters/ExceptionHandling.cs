using FluentValidation;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Errors;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Portal.Filters;

internal class ExceptionHandling : ExceptionFilterAttribute
{
  private static readonly Dictionary<Type, Func<ExceptionContext, IActionResult>> _handlers = new()
  {
    [typeof(ConfigurationAlreadyInitializedException)] = HandleConfigurationAlreadyInitializedException,
    [typeof(EmailAddressAlreadyUsedException)] = HandleEmailAddressAlreadyUsedException,
    [typeof(IncorrectSessionSecretException)] = HandleIncorrectSessionSecretException,
    [typeof(IncorrectUserPasswordException)] = HandleIncorrectUserPasswordException,
    [typeof(InvalidRefreshTokenException)] = HandleInvalidRefreshTokenException,
    [typeof(SessionIsNotActiveException)] = HandleSessionIsNotActiveException,
    [typeof(SessionIsNotPersistentException)] = HandleSessionIsNotPersistentException,
    [typeof(SessionNotFoundException)] = HandleSessionNotFoundException,
    [typeof(UserHasNoPasswordException)] = HandleUserHasNoPasswordException,
    [typeof(UserIsDisabledException)] = HandleUserIsDisabledException,
    [typeof(UserNotFoundException)] = HandleUserNotFoundException,
    [typeof(ValidationException)] = HandleValidationException
  };

  public override void OnException(ExceptionContext context)
  {
    if (_handlers.TryGetValue(context.Exception.GetType(), out Func<ExceptionContext, IActionResult>? handler))
    {
      context.Result = handler(context);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is CustomIdentifierAlreadyUsedException customIdentifierAlreadyUsed)
    {
      context.Result = new ConflictObjectResult(new Error(customIdentifierAlreadyUsed.GetErrorCode(), CustomIdentifierAlreadyUsedException.ErrorMessage)); // TODO(fpion): include Data?
      context.ExceptionHandled = true;
    }
    else if (context.Exception is IdentifierAlreadyUsedException identifierAlreadyUsed)
    {
      context.Result = new ConflictObjectResult(new Error(identifierAlreadyUsed.GetErrorCode(), IdentifierAlreadyUsedException.ErrorMessage)); // TODO(fpion): include Data?
      context.ExceptionHandled = true;
    }
    else if (context.Exception is TooManyResultsException tooManyResults)
    {
      context.Result = new ConflictObjectResult(new Error(tooManyResults.GetErrorCode(), TooManyResultsException.ErrorMessage)); // TODO(fpion): include Data?
      context.ExceptionHandled = true;
    }
    else if (context.Exception is UniqueNameAlreadyUsedException uniqueNameAlreadyUsed)
    {
      context.Result = new ConflictObjectResult(new Error(uniqueNameAlreadyUsed.GetErrorCode(), UniqueNameAlreadyUsedException.ErrorMessage)); // TODO(fpion): include Data?
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

  private static ConflictObjectResult HandleEmailAddressAlreadyUsedException(ExceptionContext context)
  {
    EmailAddressAlreadyUsedException exception = (EmailAddressAlreadyUsedException)context.Exception;
    return new ConflictObjectResult(new Error(exception.GetErrorCode(), exception.Message));
  }

  private static BadRequestObjectResult HandleIncorrectSessionSecretException(ExceptionContext context)
  {
    IncorrectSessionSecretException exception = (IncorrectSessionSecretException)context.Exception;
    return new BadRequestObjectResult(new Error(exception.GetErrorCode(), IncorrectSessionSecretException.ErrorMessage)); // TODO(fpion): include Data?
  }

  private static BadRequestObjectResult HandleIncorrectUserPasswordException(ExceptionContext context)
  {
    IncorrectUserPasswordException exception = (IncorrectUserPasswordException)context.Exception;
    return new BadRequestObjectResult(new Error(exception.GetErrorCode(), IncorrectUserPasswordException.ErrorMessage)); // TODO(fpion): include Data?
  }

  private static BadRequestObjectResult HandleInvalidRefreshTokenException(ExceptionContext context)
  {
    InvalidRefreshTokenException exception = (InvalidRefreshTokenException)context.Exception;
    return new BadRequestObjectResult(new Error(exception.GetErrorCode(), InvalidRefreshTokenException.ErrorMessage)); // TODO(fpion): include Data?
  }

  private static BadRequestObjectResult HandleSessionIsNotActiveException(ExceptionContext context)
  {
    SessionIsNotActiveException exception = (SessionIsNotActiveException)context.Exception; // TODO(fpion): does not inherit from InvalidCredentialsException
    return new BadRequestObjectResult(new Error(exception.GetErrorCode(), SessionIsNotActiveException.ErrorMessage)); // TODO(fpion): include Data?
  }

  private static BadRequestObjectResult HandleSessionIsNotPersistentException(ExceptionContext context)
  {
    SessionIsNotPersistentException exception = (SessionIsNotPersistentException)context.Exception;
    return new BadRequestObjectResult(new Error(exception.GetErrorCode(), SessionIsNotPersistentException.ErrorMessage)); // TODO(fpion): include Data?
  }

  private static BadRequestObjectResult HandleSessionNotFoundException(ExceptionContext context)
  {
    SessionNotFoundException exception = (SessionNotFoundException)context.Exception;
    return new BadRequestObjectResult(new Error(exception.GetErrorCode(), SessionNotFoundException.ErrorMessage)); // TODO(fpion): include Data?
  }

  private static BadRequestObjectResult HandleUserHasNoPasswordException(ExceptionContext context)
  {
    UserHasNoPasswordException exception = (UserHasNoPasswordException)context.Exception;
    return new BadRequestObjectResult(new Error(exception.GetErrorCode(), UserHasNoPasswordException.ErrorMessage)); // TODO(fpion): include Data?
  }

  private static BadRequestObjectResult HandleUserIsDisabledException(ExceptionContext context)
  {
    UserIsDisabledException exception = (UserIsDisabledException)context.Exception;
    return new BadRequestObjectResult(new Error(exception.GetErrorCode(), UserIsDisabledException.ErrorMessage)); // TODO(fpion): include Data?
  }

  private static BadRequestObjectResult HandleUserNotFoundException(ExceptionContext context)
  {
    UserNotFoundException exception = (UserNotFoundException)context.Exception;
    return new BadRequestObjectResult(new Error(exception.GetErrorCode(), UserNotFoundException.ErrorMessage)); // TODO(fpion): include Data?
  }

  private static BadRequestObjectResult HandleValidationException(ExceptionContext context)
  {
    ValidationException exception = (ValidationException)context.Exception;
    return new BadRequestObjectResult(new ValidationError(exception));
  }
}
