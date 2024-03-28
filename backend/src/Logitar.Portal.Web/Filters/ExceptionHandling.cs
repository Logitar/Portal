using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Application.Messages;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Application.Templates;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.Infrastructure;
using Logitar.Portal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace Logitar.Portal.Web.Filters;

internal class ExceptionHandling : ExceptionFilterAttribute
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
    if (exception is ValidationException validationException)
    {
      context.Result = new BadRequestObjectResult(new ValidationError(validationException));
      context.ExceptionHandled = true;
    }
    else if (IsBadRequestError(exception))
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

  private static readonly HashSet<Type> _badRequestExceptions = new(
  [
    typeof(CannotDeleteDefaultSenderException),
    typeof(InvalidSmsMessageContentTypeException),
    typeof(MissingRecipientContactsException),
    typeof(NoDefaultSenderException),
    typeof(SenderNotInTenantException),
    typeof(SenderProviderMismatchException),
    typeof(SenderProviderNotSupportedException),
    typeof(TemplateNotInTenantException),
    typeof(ToRecipientMissingException),
    typeof(TooManyHeaderValuesException),
    typeof(UsersNotInTenantException)
  ]);
  private static bool IsBadRequestError(Exception exception) => _badRequestExceptions.Contains(exception.GetType())
    || exception is InvalidCredentialsException || exception is SecurityTokenException || exception is TooManyResultsException;

  private static readonly HashSet<Type> _conflictExceptions = new(
  [
    typeof(ConfigurationAlreadyInitializedException),
    typeof(DictionaryAlreadyExistsException),
    typeof(EmailAddressAlreadyUsedException),
    typeof(UniqueKeyAlreadyUsedException),
    typeof(UniqueSlugAlreadyUsedException)
  ]);
  private static bool IsConflictError(Exception exception) => _conflictExceptions.Contains(exception.GetType())
    || exception is CustomIdentifierAlreadyUsedException || exception is UniqueNameAlreadyUsedException;

  private static readonly HashSet<Type> _notFoundExceptions = new(
  [
    typeof(ImpersonifiedUserNotFoundException),
    typeof(RealmNotFoundException),
    typeof(RolesNotFoundException),
    typeof(SenderNotFoundException),
    typeof(TemplateNotFoundException),
    typeof(UsersNotFoundException)
  ]);
  private static bool IsNotFoundError(Exception exception) => _notFoundExceptions.Contains(exception.GetType());

  private static string GetErrorMessage(Exception exception) => exception.Message.Remove("\r").Split('\n').First();
}
