using FluentValidation;
using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Realms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Portal.Filters;

internal class PortalExceptionFilterAttribute : ExceptionFilterAttribute
{
  private readonly Dictionary<Type, Action<ExceptionContext>> _handlers = new()
  {
    [typeof(ConfigurationAlreadyInitializedException)] = HandleConfigurationAlreadyInitializedException,
    [typeof(EmailAddressAlreadyUsedException)] = HandleEmailAddressAlreadyUsedException,
    [typeof(InvalidAggregateIdException)] = HandleInvalidAggregateIdException,
    [typeof(InvalidGenderException)] = HandleInvalidGenderException,
    [typeof(InvalidLocaleException)] = HandleInvalidLocaleException,
    [typeof(InvalidTimeZoneEntryException)] = HandleInvalidTimeZoneEntryException,
    [typeof(InvalidUrlException)] = HandleInvalidUrlException,
    [typeof(UniqueNameAlreadyUsedException)] = HandleUniqueNameAlreadyUsedException,
    [typeof(UniqueSlugAlreadyUsedException)] = HandleUniqueSlugAlreadyUsedException,
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
      context.Result = new NotFoundObjectResult(exception.Failure);
      context.ExceptionHandled = true;
    }
  }

  private static void HandleConfigurationAlreadyInitializedException(ExceptionContext context)
  {
    context.Result = new JsonResult(new { Code = "ConfigurationAlreadyInitialized" })
    {
      StatusCode = StatusCodes.Status403Forbidden
    };
  }

  private static void HandleEmailAddressAlreadyUsedException(ExceptionContext context)
  {
    context.Result = new ConflictObjectResult(((EmailAddressAlreadyUsedException)context.Exception).Failure);
  }

  private static void HandleInvalidAggregateIdException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(((InvalidAggregateIdException)context.Exception).Failure);
  }

  private static void HandleInvalidGenderException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(((InvalidGenderException)context.Exception).Failure);
  }

  private static void HandleInvalidLocaleException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(((InvalidLocaleException)context.Exception).Failure);
  }

  private static void HandleInvalidTimeZoneEntryException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(((InvalidTimeZoneEntryException)context.Exception).Failure);
  }

  private static void HandleInvalidUrlException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(((InvalidUrlException)context.Exception).Failure);
  }

  private static void HandleUniqueNameAlreadyUsedException(ExceptionContext context)
  {
    context.Result = new ConflictObjectResult(((UniqueNameAlreadyUsedException)context.Exception).Failure);
  }

  private static void HandleUniqueSlugAlreadyUsedException(ExceptionContext context)
  {
    context.Result = new ConflictObjectResult(((UniqueSlugAlreadyUsedException)context.Exception).Failure);
  }

  private static void HandleValidationException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(new { ((ValidationException)context.Exception).Errors });
  }
}
