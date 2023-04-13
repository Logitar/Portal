namespace Logitar.Portal.Core.Logging;

public interface ILoggingService
{
  Task StartAsync(string? correlationId = null, string? method = null, string? destination = null,
    string? source = null, string? additionalInformation = null);
}
