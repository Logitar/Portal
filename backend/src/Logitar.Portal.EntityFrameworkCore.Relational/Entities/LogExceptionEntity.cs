namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal class LogExceptionEntity
{
  public long LogExceptionId { get; private set; }

  public LogEntity? Log { get; private set; }
  public long LogId { get; private set; }

  public string Type { get; private set; } = string.Empty;
  public string Message { get; private set; } = string.Empty;

  public int HResult { get; private set; }
  public string? HelpLink { get; private set; }
  public string? Source { get; private set; }
  public string? StackTrace { get; private set; }
  public string? TargetSite { get; private set; }

  public string? Data { get; private set; }

  public LogExceptionEntity(LogEntity log, Exception exception, JsonSerializerOptions? serializerOptions = null)
  {
    Log = log;
    LogId = log.LogId;

    Type = exception.GetType().GetNamespaceQualifiedName();
    Message = exception.Message;

    HResult = exception.HResult;
    HelpLink = exception.HelpLink;
    Source = exception.Source;
    StackTrace = exception.StackTrace;
    TargetSite = exception.TargetSite?.ToString();

    foreach (object key in exception.Data.Keys)
    {
      try
      {
        object? value = exception.Data[key];
        if (value != null)
        {
          string serializedKey = JsonSerializer.Serialize(key, key.GetType(), serializerOptions).Trim('"');
          string serializedValue = JsonSerializer.Serialize(value, value.GetType(), serializerOptions).Trim('"');
          //Data[serializedKey] = serializedValue; // TODO(fpion): implement
        }
      }
      catch (Exception)
      {
      }
    }
  }

  private LogExceptionEntity()
  {
  }

  public override bool Equals(object? obj) => obj is LogExceptionEntity entity && entity.LogExceptionId == LogExceptionId;
  public override int GetHashCode() => LogExceptionId.GetHashCode();
  public override string ToString() => $"{GetType()} (LogExceptionId={LogExceptionId})";
}
