using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Application.Messages;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Application.Templates;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.Web;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;

namespace Logitar.Portal;

internal class ExceptionHandler : IExceptionHandler
{
  private readonly ProblemDetailsFactory _problemDetailsFactory;
  private readonly IProblemDetailsService _problemDetailsService;
  private readonly JsonSerializerOptions _serializerOptions = new();

  public ExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IProblemDetailsService problemDetailsService)
  {
    _problemDetailsFactory = problemDetailsFactory;
    _problemDetailsService = problemDetailsService;
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
  {
    int? statusCode = null;
    if (IsBadRequestException(exception))
    {
      statusCode = StatusCodes.Status400BadRequest;
    }
    else if (IsNotFoundError(exception))
    {
      statusCode = StatusCodes.Status404NotFound;
    }
    else if (IsConflictException(exception))
    {
      statusCode = StatusCodes.Status409Conflict;
    }

    if (statusCode == null)
    {
      return false;
    }

    string code = exception.GetErrorCode();
    string message = exception is ValidationException ? "Validation failed." : exception.Message.Remove("\r").Split('\n').First();
    Dictionary<string, object?> data = new(capacity: exception.Data.Count);
    foreach (DictionaryEntry item in exception.Data)
    {
      try
      {
        string? key = item.Key is string keyString ? keyString : JsonSerializer.Serialize(item.Key, item.Key.GetType(), _serializerOptions);
        data[key] = item.Value;
      }
      catch (Exception)
      {
      }
    }

    ProblemDetails problemDetails = _problemDetailsFactory.CreateProblemDetails(
      httpContext,
      statusCode,
      title: FormatToTitle(exception.GetErrorCode()),
      type: null,
      detail: message,
      instance: httpContext.Request.GetDisplayUrl());

    problemDetails.Extensions.TryAdd("code", code);
    problemDetails.Extensions.TryAdd("message", message);
    if (data.Count > 0)
    {
      problemDetails.Extensions.TryAdd("data", data);
    }
    if (exception is ValidationException validation)
    {
      problemDetails.Extensions.TryAdd("errors", validation.Errors);
    }

    httpContext.Response.StatusCode = statusCode.Value;
    ProblemDetailsContext context = new()
    {
      HttpContext = httpContext,
      ProblemDetails = problemDetails,
      Exception = exception
    };
    return await _problemDetailsService.TryWriteAsync(context);
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
    typeof(TenantMismatchException),
    typeof(ToRecipientMissingException),
    typeof(TooManyHeaderValuesException),
    typeof(UsersNotInTenantException),
    typeof(ValidationException)
  ]);
  private static bool IsBadRequestException(Exception exception) => _badRequestExceptions.Contains(exception.GetType())
    || exception is InvalidCredentialsException || exception is SecurityTokenException || exception is TooManyResultsException;

  private static readonly HashSet<Type> _conflictExceptions = new(
  [
    typeof(ConfigurationAlreadyInitializedException),
    typeof(CustomIdentifierAlreadyUsedException),
    typeof(DictionaryAlreadyExistsException),
    typeof(EmailAddressAlreadyUsedException),
    typeof(IdAlreadyUsedException),
    typeof(UniqueKeyAlreadyUsedException),
    typeof(UniqueNameAlreadyUsedException),
    typeof(UniqueSlugAlreadyUsedException)
  ]);
  private static bool IsConflictException(Exception exception) => _conflictExceptions.Contains(exception.GetType());

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

  private static string FormatToTitle(string code)
  {
    List<string> words = new(capacity: code.Length);

    StringBuilder word = new();
    for (int i = 0; i < code.Length; i++)
    {
      char? previous = (i > 0) ? code[i - 1] : null;
      char current = code[i];
      char? next = (i < code.Length - 1) ? code[i + 1] : null;

      if (char.IsUpper(current) && ((previous.HasValue && char.IsLower(previous.Value)) || (next.HasValue && char.IsLower(next.Value))))
      {
        if (word.Length > 0)
        {
          words.Add(word.ToString());
          word.Clear();
        }
      }

      word.Append(current);
    }
    if (word.Length > 0)
    {
      words.Add(word.ToString());
    }

    return string.Join(' ', words);
  }
}
