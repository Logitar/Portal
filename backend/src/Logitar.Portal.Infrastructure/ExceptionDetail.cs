namespace Logitar.Portal.Infrastructure;

public record ExceptionDetail
{
  public string Type { get; init; } = string.Empty;
  public string Message { get; init; } = string.Empty;
  public ExceptionDetail? InnerException { get; init; }

  public int HResult { get; init; }
  public string? HelpLink { get; init; }
  public string? Source { get; init; }
  public string? StackTrace { get; init; }
  public string? TargetSite { get; init; }

  public List<DictionaryEntry> Data { get; init; } = [];

  [JsonConstructor]
  public ExceptionDetail()
  {
  }

  public ExceptionDetail(Exception exception)
  {
    Type = exception.GetType().GetLongestName();
    Message = exception.Message;
    if (exception.InnerException != null)
    {
      InnerException = new ExceptionDetail(exception.InnerException);
    }

    HResult = exception.HResult;
    HelpLink = exception.HelpLink;
    Source = exception.Source;
    StackTrace = exception.StackTrace;
    TargetSite = exception.TargetSite?.ToString();

    foreach (DictionaryEntry entry in exception.Data)
    {
      if (IsSerializable(entry.Key) && (entry.Value == null || IsSerializable(entry.Value)))
      {
        Data.Add(entry);
      }
    }
  }

  private static bool IsSerializable(object instance)
  {
    try
    {
      _ = JsonSerializer.Serialize(instance, instance.GetType());
      return true;
    }
    catch (Exception)
    {
      return false;
    }
  }
}
