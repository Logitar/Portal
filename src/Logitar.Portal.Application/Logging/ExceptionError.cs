namespace Logitar.Portal.Application.Logging;

public record ExceptionError : Error
{
  public ExceptionError(Exception exception, ErrorSeverity severity = ErrorSeverity.Failure)
    : base()
  {
    Severity = severity;
    Code = exception.GetType().Name.Remove(nameof(Exception));
    Description = exception.Message;

    HelpLink = exception.HelpLink;
    HResult = exception.HResult;
    InnerException = exception.InnerException == null ? null : new ExceptionError(exception.InnerException, severity);
    Source = exception.Source;
    StackTrace = exception.StackTrace;
    TargetSite = exception.TargetSite?.ToString();

    Dictionary<string, string> data = new(capacity: exception.Data.Count);
    foreach (object key in exception.Data.Keys)
    {
      object? value = exception.Data[key];
      if (value != null)
      {
        string serializedKey = JsonSerializer.Serialize(key, key.GetType(), SerializerOptions);
        string serializedValue = JsonSerializer.Serialize(value, value.GetType(), SerializerOptions);

        data[serializedKey] = serializedValue;
      }
    }
    Data = data.AsReadOnly();
  }

  public IReadOnlyDictionary<string, string> Data { get; }
  public string? HelpLink { get; }
  public int HResult { get; }
  public ExceptionError? InnerException { get; }
  public string? Source { get; }
  public string? StackTrace { get; }
  public string? TargetSite { get; }
}
