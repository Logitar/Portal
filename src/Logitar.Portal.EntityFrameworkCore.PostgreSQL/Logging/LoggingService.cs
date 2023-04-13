using Logitar.Portal.Core.Logging;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Logging;

internal class LoggingService : ILoggingService
{
  private LogEntity? _log = null;

  public Task StartAsync(string? correlationId, string? method, string? destination,
    string? source, string? additionalInformation)
  {
    if (_log != null)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }

    _log = new(correlationId, method, destination, source, additionalInformation);

    return Task.CompletedTask;
  }
}
