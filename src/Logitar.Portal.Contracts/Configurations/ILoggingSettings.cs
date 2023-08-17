namespace Logitar.Portal.Contracts.Configurations;

public interface ILoggingSettings
{
  LoggingExtent Extent { get; }
  bool OnlyErrors { get; }
}
