using FluentValidation;
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
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace Logitar.Portal.Web.Filters;

internal class ExceptionHandlingFilter : ExceptionFilterAttribute
{
  private readonly Dictionary<Type, Func<ExceptionContext, IActionResult>> _handlers = new()
  {
    [typeof(ApiKeyIsExpiredException)] = HandleBadRequestDetailException,
    [typeof(CannotDeleteDefaultSenderException)] = HandleBadRequestDetailException,
    [typeof(CannotPostponeExpirationException)] = HandleBadRequestFailureException,
    [typeof(ConfigurationAlreadyInitializedException)] = HandleConfigurationAlreadyInitializedException,
    [typeof(DictionaryAlreadyExistingException)] = HandleBadRequestDetailException,
    [typeof(EmailAddressAlreadyUsedException)] = HandleBadRequestDetailException,
    [typeof(InvalidGenderException)] = HandleBadRequestFailureException,
    [typeof(InvalidLocaleException)] = HandleBadRequestFailureException,
    [typeof(InvalidTimeZoneEntryException)] = HandleBadRequestFailureException,
    [typeof(InvalidUrlException)] = HandleBadRequestFailureException,
    [typeof(MissingRecipientAddressesException)] = HandleBadRequestFailureException,
    [typeof(MissingToRecipientException)] = HandleBadRequestDetailException,
    [typeof(RealmHasNoDefaultSenderException)] = HandleBadRequestFailureException,
    [typeof(RealmHasNoPasswordRecoveryTemplateException)] = HandleBadRequestDetailException,
    [typeof(RolesNotFoundException)] = HandleNotFoundFailureException,
    [typeof(SenderNotInRealmException)] = HandleBadRequestFailureException,
    [typeof(SessionIsNotActiveException)] = HandleBadRequestDetailException,
    [typeof(TemplateNotInRealmException)] = HandleBadRequestFailureException,
    [typeof(UniqueSlugAlreadyUsedException)] = HandleBadRequestDetailException,
    [typeof(UserIsDisabledException)] = HandleBadRequestDetailException,
    [typeof(UserIsNotConfirmedException)] = HandleBadRequestDetailException,
    [typeof(UsersNotFoundException)] = HandleNotFoundFailureException,
    [typeof(UsersNotInRealmException)] = HandleBadRequestFailureException,
    [typeof(ValidationException)] = HandleValidationException
  };

  public override void OnException(ExceptionContext context)
  {
    if (_handlers.TryGetValue(context.Exception.GetType(), out Func<ExceptionContext, IActionResult>? handler))
    {
      context.Result = handler(context);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is AggregateNotFoundException)
    {
      context.Result = HandleNotFoundFailureException(context);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is IdentifierAlreadyUsedException)
    {
      context.Result = HandleConflictFailureException(context);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is InvalidCredentialsException)
    {
      context.Result = HandleBadRequestDetailException(context);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is SecurityTokenException)
    {
      context.Result = HandleBadRequestDetailException(context);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is TooManyResultsException)
    {
      context.Result = HandleBadRequestDetailException(context);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is UniqueNameAlreadyUsedException)
    {
      context.Result = HandleConflictFailureException(context);
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }
  }

  private static IActionResult HandleBadRequestDetailException(ExceptionContext context)
  {
    return new BadRequestObjectResult(ErrorDetail.From(context.Exception));
  }
  private static IActionResult HandleBadRequestFailureException(ExceptionContext context)
  {
    return new BadRequestObjectResult(((IFailureException)context.Exception).Failure);
  }

  private static IActionResult HandleConfigurationAlreadyInitializedException(ExceptionContext context)
  {
    return new JsonResult(ErrorDetail.From(context.Exception))
    {
      StatusCode = StatusCodes.Status403Forbidden
    };
  }

  private static IActionResult HandleConflictFailureException(ExceptionContext context)
  {
    return new ConflictObjectResult(((IFailureException)context.Exception).Failure);
  }

  private static IActionResult HandleNotFoundFailureException(ExceptionContext context)
  {
    return new NotFoundObjectResult(((IFailureException)context.Exception).Failure);
  }

  private static IActionResult HandleValidationException(ExceptionContext context)
  {
    return new BadRequestObjectResult(new { ((ValidationException)context.Exception).Errors });
  }
}
