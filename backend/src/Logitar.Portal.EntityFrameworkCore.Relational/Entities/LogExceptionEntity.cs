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

    Dictionary<string, string> data = GetData();
    foreach (object key in exception.Data.Keys)
    {
      try
      {
        object? value = exception.Data[key];
        if (value != null)
        {
          string serializedKey = JsonSerializer.Serialize(key, key.GetType(), serializerOptions).Trim('"');
          string serializedValue = JsonSerializer.Serialize(value, value.GetType(), serializerOptions).Trim('"');
          data[serializedKey] = serializedValue;
        }
      }
      catch (Exception)
      {
      }
    }
    SetData(data);
  }

  private LogExceptionEntity()
  {
  }

  public Dictionary<string, string> GetData()
  {
    return (Data == null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(Data)) ?? [];
  }
  private void SetData(Dictionary<string, string> data)
  {
    Data = data.Count < 1 ? null : JsonSerializer.Serialize(data);
  }
}
