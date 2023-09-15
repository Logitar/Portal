using Logitar.Portal.Contracts.Http;
using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Infrastructure.Messages;

internal static class SendMessageResultExtensions
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static SendMessageResultExtensions()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public static SendMessageResult ToSendMessageResult(this Exception exception)
  {
    Dictionary<string, string> data = new(capacity: 9);

    Dictionary<string, string> exceptionData = new(capacity: exception.Data.Count);
    foreach (object key in exception.Data.Keys)
    {
      object? value = exception.Data[key];
      if (value != null)
      {
        string serializedKey = key as string ?? JsonSerializer.Serialize(key, key.GetType(), _serializerOptions);
        string serializedValue = value as string ?? JsonSerializer.Serialize(value, value.GetType(), _serializerOptions);

        exceptionData[serializedKey] = serializedValue;
      }
    }
    if (exceptionData.Any())
    {
      data["Data"] = JsonSerializer.Serialize(exceptionData, _serializerOptions);
    }

    if (exception.HelpLink != null)
    {
      data["HelpLink"] = exception.HelpLink;
    }

    data["HResult"] = exception.HResult.ToString();

    if (exception.InnerException != null)
    {
      Dictionary<string, string> innerExceptionData = exception.InnerException.ToSendMessageResult().AsDictionary();
      data["InnerException"] = JsonSerializer.Serialize(innerExceptionData, _serializerOptions);
    }

    data["Message"] = exception.Message;

    if (exception.Source != null)
    {
      data["Source"] = exception.Source;
    }

    if (exception.StackTrace != null)
    {
      data["StackTrace"] = exception.StackTrace;
    }

    string? targetSite = exception.TargetSite?.ToString();
    if (targetSite != null)
    {
      data["TargetSite"] = targetSite;
    }

    data["Type"] = exception.GetType().Name;

    return new SendMessageResult(data);
  }

  public static SendMessageResult ToSendMessageResult(this HttpResponseDetail detail)
  {
    Dictionary<string, string> data = new(capacity: 5);

    if (detail.Content != null)
    {
      data["Content"] = detail.Content;
    }

    if (detail.ReasonPhrase != null)
    {
      data["ReasonPhrase"] = detail.ReasonPhrase;
    }

    data["StatusCode"] = detail.StatusCode.ToString();
    data["StatusText"] = detail.StatusText;
    data["Version"] = detail.Version;

    return new SendMessageResult(data);
  }
}
