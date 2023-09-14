﻿using FluentValidation;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Application.Messages;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace Logitar.Portal.Web.Filters;

/// <summary>
/// TODO(fpion): refactor
/// </summary>
internal class ExceptionHandlingFilter : ExceptionFilterAttribute
{
  private readonly Dictionary<Type, Func<ExceptionContext, IActionResult>> _handlers = new()
  {
    [typeof(ApiKeyIsExpiredException)] = HandleApiKeyIsExpiredException,
    [typeof(CannotDeleteDefaultSenderException)] = HandleCannotDeleteDefaultSenderException,
    [typeof(CannotPostponeExpirationException)] = HandleCannotPostponeExpirationException,
    [typeof(ConfigurationAlreadyInitializedException)] = HandleConfigurationAlreadyInitializedException,
    [typeof(DictionaryAlreadyExistingException)] = HandleDictionaryAlreadyExistingException,
    [typeof(EmailAddressAlreadyUsedException)] = HandleEmailAddressAlreadyUsedException,
    [typeof(InvalidGenderException)] = HandleInvalidGenderException,
    [typeof(InvalidLocaleException)] = HandleInvalidLocaleException,
    [typeof(InvalidTimeZoneEntryException)] = HandleInvalidTimeZoneEntryException,
    [typeof(InvalidUrlException)] = HandleInvalidUrlException,
    [typeof(MissingRecipientAddressesException)] = HandleMissingRecipientAddressesException,
    [typeof(RealmHasNoDefaultSenderException)] = HandleRealmHasNoDefaultSenderException,
    [typeof(RolesNotFoundException)] = HandleRolesNotFoundException,
    [typeof(SenderNotInRealmException)] = HandleSenderNotInRealmException,
    [typeof(SessionIsNotActiveException)] = HandleSessionIsNotActiveException,
    [typeof(TemplateNotInRealmException)] = HandleTemplateNotInRealmException,
    [typeof(UniqueSlugAlreadyUsedException)] = HandleUniqueSlugAlreadyUsedException,
    [typeof(UserIsDisabledException)] = HandleUserIsDisabledException,
    [typeof(UserIsNotConfirmedException)] = HandleUserIsNotConfirmedException,
    [typeof(UsersNotFoundException)] = HandleUsersNotFoundException,
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
      context.Result = new BadRequestObjectResult(ErrorDetail.From(invalidCredentials));
      context.ExceptionHandled = true;
    }
    else if (context.Exception is SecurityTokenException securityToken)
    {
      context.Result = new BadRequestObjectResult(ErrorDetail.From(securityToken));
      context.ExceptionHandled = true;
    }
    else if (context.Exception is TooManyResultsException tooManyResults)
    {
      context.Result = new BadRequestObjectResult(ErrorDetail.From(tooManyResults));
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
    return new BadRequestObjectResult(ErrorDetail.From(context.Exception));
  }

  private static IActionResult HandleCannotDeleteDefaultSenderException(ExceptionContext context)
  {
    return new BadRequestObjectResult(ErrorDetail.From(context.Exception));
  }

  private static IActionResult HandleCannotPostponeExpirationException(ExceptionContext context)
  {
    return new BadRequestObjectResult(((CannotPostponeExpirationException)context.Exception).Failure);
  }

  private static IActionResult HandleConfigurationAlreadyInitializedException(ExceptionContext context)
  {
    return new JsonResult(ErrorDetail.From(context.Exception))
    {
      StatusCode = StatusCodes.Status403Forbidden
    };
  }

  private static IActionResult HandleDictionaryAlreadyExistingException(ExceptionContext context)
  {
    return new ConflictObjectResult(((DictionaryAlreadyExistingException)context.Exception).Failure);
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

  private static IActionResult HandleMissingRecipientAddressesException(ExceptionContext context)
  {
    return new BadRequestObjectResult(((MissingRecipientAddressesException)context.Exception).Failure);
  }

  private static IActionResult HandleRealmHasNoDefaultSenderException(ExceptionContext context)
  {
    return new BadRequestObjectResult(((RealmHasNoDefaultSenderException)context.Exception).Failure);
  }

  private static IActionResult HandleRolesNotFoundException(ExceptionContext context)
  {
    return new NotFoundObjectResult(((RolesNotFoundException)context.Exception).Failure);
  }

  private static IActionResult HandleSenderNotInRealmException(ExceptionContext context)
  {
    return new BadRequestObjectResult(((SenderNotInRealmException)context.Exception).Failure);
  }

  private static IActionResult HandleSessionIsNotActiveException(ExceptionContext context)
  {
    return new BadRequestObjectResult(ErrorDetail.From(context.Exception));
  }

  private static IActionResult HandleTemplateNotInRealmException(ExceptionContext context)
  {
    return new BadRequestObjectResult(((TemplateNotInRealmException)context.Exception).Failure);
  }

  private static IActionResult HandleUniqueSlugAlreadyUsedException(ExceptionContext context)
  {
    return new ConflictObjectResult(((UniqueSlugAlreadyUsedException)context.Exception).Failure);
  }

  private static IActionResult HandleUserIsDisabledException(ExceptionContext context)
  {
    return new BadRequestObjectResult(ErrorDetail.From(context.Exception));
  }

  private static IActionResult HandleUserIsNotConfirmedException(ExceptionContext context)
  {
    return new BadRequestObjectResult(ErrorDetail.From(context.Exception));
  }

  private static IActionResult HandleUsersNotFoundException(ExceptionContext context)
  {
    return new NotFoundObjectResult(((UsersNotFoundException)context.Exception).Failure);
  }

  private static IActionResult HandleValidationException(ExceptionContext context)
  {
    return new BadRequestObjectResult(new { ((ValidationException)context.Exception).Errors });
  }
}
