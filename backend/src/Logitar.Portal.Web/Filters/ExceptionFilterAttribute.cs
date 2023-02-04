using FluentValidation;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Application.Messages;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Application.Templates;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Portal.Web.Filters
{
  public class ExceptionFilterAttribute : Microsoft.AspNetCore.Mvc.Filters.ExceptionFilterAttribute
  {
    private static readonly Dictionary<Type, Action<ExceptionContext>> _handlers = new()
    {
      // 400 BadRequest
      [typeof(AccountIsDisabledException)] = SetBadRequestCodeResult,
      [typeof(AccountNotConfirmedException)] = SetBadRequestCodeResult,
      [typeof(CannotDeleteDefaultSenderException)] = SetBadRequestCodeResult,
      [typeof(CannotDeletePasswordRecoverySenderException)] = SetBadRequestCodeResult,
      [typeof(CannotDeletePasswordRecoveryTemplateException)] = SetBadRequestCodeResult,
      [typeof(DefaultSenderRequiredException)] = SetBadRequestCodeResult,
      [typeof(GoogleClientIdRequiredException)] = SetBadRequestCodeResult,
      [typeof(InvalidCredentialsException)] = SetBadRequestCodeResult,
      [typeof(SenderNotInRealmException)] = HandleSenderNotInRealmException,
      [typeof(SessionAlreadySignedOutException)] = SetBadRequestCodeResult,
      [typeof(SessionIsNotActiveException)] = SetBadRequestCodeResult,
      [typeof(TemplateNotInRealmException)] = HandleTemplateNotInRealmException,
      [typeof(UserAlreadyDisabledException)] = SetBadRequestCodeResult,
      [typeof(UserCannotDeleteItselfException)] = SetBadRequestCodeResult,
      [typeof(UserCannotDisableItselfException)] = SetBadRequestCodeResult,
      [typeof(UserEmailRequiredException)] = SetBadRequestCodeResult,
      [typeof(UserHasNoPasswordException)] = SetBadRequestCodeResult,
      [typeof(UserNotDisabledException)] = SetBadRequestCodeResult,
      [typeof(UsersEmailRequiredException)] = HandleUsersEmailRequiredException,
      [typeof(UsersNotInRealmException)] = HandleUsersNotInRealmException,
      [typeof(ValidationException)] = HandleValidationException,
      // 403 Forbidden
      [typeof(ConfigurationAlreadyInitializedException)] = context => context.Result = new ForbidResult(),
      // 404 NotFound
      [typeof(UsersNotFoundException)] = HandleUsersNotFoundException,
      // 409 Conflict
      [typeof(AliasAlreadyUsedException)] = SetConflictFieldResult,
      [typeof(DictionaryAlreadyExistingException)] = HandleDictionaryAlreadyExistingException,
      [typeof(EmailAlreadyUsedException)] = SetConflictFieldResult,
      [typeof(KeyAlreadyUsedException)] = SetConflictFieldResult,
      [typeof(UsernameAlreadyUsedException)] = SetConflictFieldResult
    };

    public override void OnException(ExceptionContext context)
    {
      if (_handlers.TryGetValue(context.Exception.GetType(), out Action<ExceptionContext>? handler))
      {
        handler(context);
        context.ExceptionHandled = true;
      }
      else if (context.Exception is EntityNotFoundException entityNotFound)
      {
        context.Result = entityNotFound.Data["ParamName"] == null
          ? new NotFoundResult()
          : new NotFoundObjectResult(new { field = entityNotFound.Data["ParamName"] });
        context.ExceptionHandled = true;
      }
    }

    private static void SetBadRequestCodeResult(ExceptionContext context) => context.Result = new BadRequestObjectResult(new
    {
      code = GetCode(context)
    });
    private static void SetConflictFieldResult(ExceptionContext context) => context.Result = new ConflictObjectResult(new
    {
      field = context.Exception.Data["ParamName"]
    });

    private static void HandleDictionaryAlreadyExistingException(ExceptionContext context) => context.Result = new ConflictObjectResult(new
    {
      realm = context.Exception.Data["Realm"],
      locale = context.Exception.Data["Locale"]
    });

    private static void HandleSenderNotInRealmException(ExceptionContext context) => context.Result = new BadRequestObjectResult(new Dictionary<string, object?>
    {
      ["code"] = GetCode(context),
      ["realm"] = context.Exception.Data["Realm"],
      [(string?)context.Exception.Data["ParamName"] ?? "Sender"] = context.Exception.Data["Sender"]
    });

    private static void HandleTemplateNotInRealmException(ExceptionContext context) => context.Result = new BadRequestObjectResult(new Dictionary<string, object?>
    {
      ["code"] = GetCode(context),
      ["realm"] = context.Exception.Data["Realm"],
      [(string?)context.Exception.Data["ParamName"] ?? "Template"] = context.Exception.Data["Template"]
    });

    private static void HandleUsersEmailRequiredException(ExceptionContext context) => context.Result = new BadRequestObjectResult(new Dictionary<string, object?>
    {
      ["code"] = GetCode(context),
      [(string?)context.Exception.Data["ParamName"] ?? "Ids"] = context.Exception.Data["Ids"]
    });

    private static void HandleUsersNotInRealmException(ExceptionContext context) => context.Result = new BadRequestObjectResult(new Dictionary<string, object?>
    {
      ["code"] = GetCode(context),
      ["realm"] = context.Exception.Data["Realm"],
      [(string?)context.Exception.Data["ParamName"] ?? "Ids"] = context.Exception.Data["Ids"]
    });

    private static void HandleUsersNotFoundException(ExceptionContext context) => context.Result = new NotFoundObjectResult(new Dictionary<string, object?>
    {
      ["code"] = GetCode(context),
      [(string?)context.Exception.Data["ParamName"] ?? "Ids"] = context.Exception.Data["Ids"]
    });

    private static void HandleValidationException(ExceptionContext context) => context.Result = new BadRequestObjectResult(new
    {
      errors = ((ValidationException)context.Exception).Errors
    });

    private static string GetCode(ExceptionContext context) => context.Exception.GetType().Name.Remove(nameof(Exception));
  }
}
