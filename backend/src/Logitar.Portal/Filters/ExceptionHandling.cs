using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Application.Templates;
using Logitar.Portal.Application.Userss;
using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Portal.Filters;

internal class ExceptionHandling : ExceptionFilterAttribute // TODO(fpion): refactor
{
  private readonly JsonSerializerOptions _serializerOptions;

  public ExceptionHandling(IServiceProvider serviceProvider)
  {
    _serializerOptions = new JsonSerializerOptions();
    _serializerOptions.Converters.AddRange(serviceProvider.GetLogitarPortalJsonConverters());
  }

  public override void OnException(ExceptionContext context)
  {
    Exception exception = context.Exception;
    if (IsBadRequestError(exception))
    {
      context.Result = new BadRequestObjectResult(BuildError(exception));
      context.ExceptionHandled = true;
    }
    else if (IsConflictError(exception))
    {
      context.Result = new ConflictObjectResult(BuildError(exception));
      context.ExceptionHandled = true;
    }
    else if (IsNotFoundError(exception))
    {
      context.Result = new NotFoundObjectResult(BuildError(exception));
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }
  }

  private Error BuildError(Exception exception)
  {
    Error error = new(exception.GetErrorCode(), GetErrorMessage(exception));

    foreach (object key in exception.Data.Keys)
    {
      object? value = exception.Data[key];

      string keyString = JsonSerializer.Serialize(key, key.GetType(), _serializerOptions).Trim('"');
      string? valueString = value == null ? null : JsonSerializer.Serialize(value, value.GetType(), _serializerOptions).Trim('"');

      error.AddData(keyString, valueString);
    }

    return error;
  }

  private static bool IsBadRequestError(Exception exception)
  {
    return exception is CannotDeleteDefaultSenderException
      || exception is InvalidCredentialsException
      || exception is NoDefaultSenderException
      || exception is SenderNotInTenantException
      || exception is SenderProviderMismatchException
      || exception is SenderProviderNotSupportedException
      || exception is TemplateNotInTenantException
      || exception is TooManyResultsException
      || exception is ToRecipientMissingException
      || exception is UsersNotInTenantException;
  }
  private static bool IsConflictError(Exception exception)
  {
    return exception is ConfigurationAlreadyInitializedException
      || exception is DictionaryAlreadyExistsException
      || exception is UniqueKeyAlreadyUsedException
      || exception is UniqueSlugAlreadyUsedException;
  }
  private static bool IsNotFoundError(Exception exception)
  {
    return exception is RolesNotFoundException
      || exception is SenderNotFoundException
      || exception is TemplateNotFoundException
      || exception is UsersNotFoundException;
  }

  private static string GetErrorMessage(Exception exception) => exception.Message.Remove("\r").Split('\n').First();
}
