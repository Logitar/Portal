using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Dictionaries;
using Logitar.Portal.Core.Messages;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Senders;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Portal.Web.Filters;

internal class ExceptionHandlingFilter : ExceptionFilterAttribute
{
  private static readonly Dictionary<Type, Action<ExceptionContext>> _handlers = new()
  {
    [typeof(AccountIsDisabledException)] = HandleAccountIsDisabledException,
    [typeof(AccountIsNotConfirmedException)] = HandleAccountIsNotConfirmedException,
    [typeof(CannotDeleteDefaultSenderException)] = HandleCannotDeleteDefaultSenderException,
    [typeof(DefaultSenderRequiredException)] = HandleDefaultSenderRequiredException,
    [typeof(EmailAddressAlreadyUsedException)] = HandleEmailAddressAlreadyUsedException,
    [typeof(ExternalIdentifierAlreadyUsedException)] = HandleExternalIdentifierAlreadyUsedException,
    [typeof(InvalidCredentialsException)] = HandleInvalidCredentialsException,
    [typeof(InvalidLocaleException)] = HandleInvalidLocaleException,
    [typeof(InvalidTimeZoneEntryException)] = HandleInvalidTimeZoneEntryException,
    [typeof(InvalidUrlException)] = HandleInvalidUrlException,
    [typeof(LocaleAlreadyUsedException)] = HandleLocaleAlreadyUsedException,
    [typeof(PasswordRecoveryTemplateRequiredException)] = HandlePasswordRecoveryTemplateRequiredException,
    [typeof(SenderNotInRealmException)] = HandleSenderNotInRealmException,
    [typeof(SessionIsNotActiveException)] = HandleSessionIsNotActiveException,
    [typeof(TemplateNotInRealmException)] = HandleTemplateNotInRealmException,
    [typeof(TooManyResultsException)] = HandleTooManyResultsException,
    [typeof(UniqueNameAlreadyUsedException)] = HandleUniqueNameAlreadyUsedException,
    [typeof(UsersHasNoEmailException)] = HandleUsersHasNoEmailException,
    [typeof(UsersNotFoundException)] = HandleUsersNotFoundException,
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
      context.ExceptionHandled = true;
    }
  }

  private static void HandleAccountIsDisabledException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(new { Code = GetCode(context.Exception) });
  }

  private static void HandleAccountIsNotConfirmedException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(new { Code = GetCode(context.Exception) });
  }

  private static void HandleCannotDeleteDefaultSenderException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(new { Code = GetCode(context.Exception) });
  }

  private static void HandleDefaultSenderRequiredException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(new { Code = GetCode(context.Exception) });
  }

  private static void HandleEmailAddressAlreadyUsedException(ExceptionContext context)
  {
    if (context.Exception is EmailAddressAlreadyUsedException exception)
    {
      context.Result = new ConflictObjectResult(GetPropertyFailure(exception));
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

  private static void HandleLocaleAlreadyUsedException(ExceptionContext context)
  {
    if (context.Exception is LocaleAlreadyUsedException exception)
    {
      context.Result = new ConflictObjectResult(GetPropertyFailure(exception));
    }
  }

  private static void HandlePasswordRecoveryTemplateRequiredException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(new { Code = GetCode(context.Exception) });
  }

  private static void HandleSenderNotInRealmException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(new { Code = GetCode(context.Exception) });
  }

  private static void HandleSessionIsNotActiveException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(new { Code = GetCode(context.Exception) });
  }

  private static void HandleTemplateNotInRealmException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(new { Code = GetCode(context.Exception) });
  }

  private static void HandleTooManyResultsException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(new { Code = GetCode(context.Exception) });
  }

  private static void HandleUniqueNameAlreadyUsedException(ExceptionContext context)
  {
    if (context.Exception is UniqueNameAlreadyUsedException exception)
    {
      context.Result = new ConflictObjectResult(GetPropertyFailure(exception));
    }
  }

  private static void HandleUsersHasNoEmailException(ExceptionContext context)
  {
    if (context.Exception is UsersHasNoEmailException exception)
    {
      context.Result = new BadRequestObjectResult(new
      {
        Code = GetCode(exception),
        exception.Ids,
        PropertyName = exception.ParamName
      });
    }
  }

  private static void HandleUsersNotFoundException(ExceptionContext context)
  {
    if (context.Exception is UsersNotFoundException exception)
    {
      context.Result = new NotFoundObjectResult(new
      {
        Code = GetCode(exception),
        exception.Users,
        PropertyName = exception.ParamName
      });
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
