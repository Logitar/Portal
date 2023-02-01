using FluentValidation;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Domain.Sessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Portal.Web.Filters
{
  public class ExceptionFilterAttribute : Microsoft.AspNetCore.Mvc.Filters.ExceptionFilterAttribute
  {
    private static readonly Dictionary<Type, Action<ExceptionContext>> _handlers = new()
    {
      [typeof(AccountNotConfirmedException)] = context => context.Result = new BadRequestObjectResult(new { code = "AccountNotConfirmed" }),
      [typeof(ConfigurationAlreadyInitializedException)] = context => context.Result = new ForbidResult(),
      [typeof(InvalidCredentialsException)] = context => context.Result = new BadRequestObjectResult(new { code = "InvalidCredentials" }),
      [typeof(SessionAlreadySignedOutException)] = context => context.Result = new BadRequestObjectResult(new { code = "SessionAlreadySignedOut" }),
      [typeof(ValidationException)] = context => context.Result = new BadRequestObjectResult(new { errors = ((ValidationException)context.Exception).Errors })
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
  }
}
