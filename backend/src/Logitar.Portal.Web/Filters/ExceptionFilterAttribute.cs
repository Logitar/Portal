using FluentValidation;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Application.Configurations;
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
      [typeof(GoogleClientIdRequiredException)] = SetBadRequestCodeResult,
      [typeof(InvalidCredentialsException)] = SetBadRequestCodeResult,
      [typeof(SessionAlreadySignedOutException)] = SetBadRequestCodeResult,
      [typeof(SessionIsNotActiveException)] = SetBadRequestCodeResult,
      [typeof(UserAlreadyDisabledException)] = SetBadRequestCodeResult,
      [typeof(UserCannotDeleteItselfException)] = SetBadRequestCodeResult,
      [typeof(UserCannotDisableItselfException)] = SetBadRequestCodeResult,
      [typeof(UserHasNoPasswordException)] = SetBadRequestCodeResult,
      [typeof(UserNotDisabledException)] = SetBadRequestCodeResult,
      [typeof(ValidationException)] = context => context.Result = new BadRequestObjectResult(new { errors = ((ValidationException)context.Exception).Errors }),
      // 403 Forbidden
      [typeof(ConfigurationAlreadyInitializedException)] = context => context.Result = new ForbidResult(),
      // 409 Conflict
      [typeof(AliasAlreadyUsedException)] = SetConflictFieldResult,
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
      code = context.Exception.GetType().Name.Remove(nameof(Exception))
    });
    private static void SetConflictFieldResult(ExceptionContext context) => context.Result = new ConflictObjectResult(new
    {
      field = context.Exception.Data["ParamName"]
    });
  }
}
