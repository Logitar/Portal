namespace Logitar.Portal.Domain.Configurations;

public interface ILoggingSettings
{
  LoggingExtent Extent { get; }
  bool OnlyErrors { get; }
}
