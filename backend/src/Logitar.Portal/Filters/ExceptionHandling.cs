﻿using FluentValidation;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Errors;
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
    [typeof(IncorrectUserPasswordException)] = HandleIncorrectUserPasswordException,
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
    else if (context.Exception is IdentifierAlreadyUsedException identifierAlreadyUsed)
    {
      context.Result = new ConflictObjectResult(new Error(identifierAlreadyUsed.GetErrorCode(), IdentifierAlreadyUsedException.ErrorMessage)); // TODO(fpion): include Data?
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

  private static BadRequestObjectResult HandleIncorrectUserPasswordException(ExceptionContext context)
  {
    IncorrectUserPasswordException exception = (IncorrectUserPasswordException)context.Exception;
    return new BadRequestObjectResult(new Error(exception.GetErrorCode(), IncorrectUserPasswordException.ErrorMessage)); // TODO(fpion): include Data?
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