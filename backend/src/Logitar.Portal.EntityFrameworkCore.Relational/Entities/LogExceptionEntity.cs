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

  public Dictionary<string, string> Data { get; private set; } = [];
  public string? DataSerialized
  {
    get => Data.Count > 0 ? JsonSerializer.Serialize(Data) : null;
    private set
    {
      Data.Clear();

      if (value != null)
      {
        Dictionary<string, string>? data = JsonSerializer.Deserialize<Dictionary<string, string>>(value);
        if (data != null)
        {
          Data.AddRange(data);
        }
      }
    }
  }

  public LogExceptionEntity(LogEntity log, Exception exception)
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
  }

  private LogExceptionEntity()
  {
  }
}
