using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Portal.Filters;

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
    if (IsConflictError(exception))
    {
      context.Result = new ConflictObjectResult(BuildError(exception));
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }
  }

  private Error BuildError(Exception exception)
  {
    Error error = new(GetErrorCode(exception), GetErrorMessage(exception));

    foreach (object key in exception.Data.Keys)
    {
      object? value = exception.Data[key];

      string keyString = JsonSerializer.Serialize(key, key.GetType(), _serializerOptions).Trim('"');
      string? valueString = value == null ? null : JsonSerializer.Serialize(value, value.GetType(), _serializerOptions).Trim('"');

      error.AddData(keyString, valueString);
    }

    return error;
  }

  private static bool IsConflictError(Exception exception)
  {
    return exception is ConfigurationAlreadyInitializedException;
  }

  private static string GetErrorCode(Exception exception) // TODO(fpion): fix from Logitar class library
  {
    string code = exception.GetErrorCode();
    int index = code.IndexOf('`');
    return index < 0 ? code : code[..index];
  }
  private static string GetErrorMessage(Exception exception) => exception.Message.Remove("\r").Split('\n').First();
}
