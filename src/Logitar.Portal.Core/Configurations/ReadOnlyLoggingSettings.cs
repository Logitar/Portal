namespace Logitar.Portal.Core.Configurations;

public record ReadOnlyLoggingSettings
{
  public LoggingExtent Extent { get; init; }
  public bool OnlyErrors { get; init; }

  public static ReadOnlyLoggingSettings? From(LoggingSettings? loggingSettings)
  {
    return loggingSettings == null ? null : new()
    {
      Extent = loggingSettings.Extent,
      OnlyErrors = loggingSettings.OnlyErrors
    };
  }
}
