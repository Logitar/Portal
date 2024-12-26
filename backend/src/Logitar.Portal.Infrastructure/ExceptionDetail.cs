namespace Logitar.Portal.Infrastructure;

public record ExceptionDetail
{
  public string Type { get; set; } = string.Empty;
  public string Message { get; set; } = string.Empty;
  public ExceptionDetail? InnerException { get; set; }

  public int HResult { get; set; }
  public string? HelpLink { get; set; }
  public string? Source { get; set; }
  public string? StackTrace { get; set; }
  public string? TargetSite { get; set; }

  public List<DictionaryEntry> Data { get; set; } = [];

  public ExceptionDetail()
  {
  }

  public ExceptionDetail(Exception exception)
  {
    Type = exception.GetType().GetLongestName();
    Message = exception.Message;
    if (exception.InnerException != null)
    {
      InnerException = new(exception.InnerException);
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
