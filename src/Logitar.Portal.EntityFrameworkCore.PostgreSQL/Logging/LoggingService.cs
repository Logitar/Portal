using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Core.Logging;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Logging;

internal class LoggingService : ILoggingService
{
  private LogEntity? _log = null;

  private readonly PortalContext _context;

  public LoggingService(PortalContext context)
  {
    _context = context;
  }

  public Task StartAsync(string? correlationId, string? method, string? destination,
    string? source, string? additionalInformation, DateTime? startedOn, CancellationToken cancellationToken)
  {
    if (_log != null)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }

    _log = new(correlationId, method, destination, source, additionalInformation, startedOn);

    return Task.CompletedTask;
  }

  public Task AddErrorAsync(Error error, CancellationToken cancellationToken)
  {
    if (_log == null)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }

    _log.AddError(error);

    return Task.CompletedTask;
  }

  public Task SetActorsAsync(Guid actorId, Guid? userId, Guid? sessionId, CancellationToken cancellationToken)
  {
    if (_log == null)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }

    _log.SetActors(actorId, userId, sessionId);

    return Task.CompletedTask;
  }

  public Task SetOperationAsync(string type, string name, CancellationToken cancellationToken)
  {
    if (_log == null)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }

    _log.SetOperation(type, name);

    return Task.CompletedTask;
  }

  public async Task EndAsync(int? statusCode, DateTime? endedOn, CancellationToken cancellationToken)
  {
    if (_log == null)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }

    _log.Complete(statusCode, endedOn);

    _context.Logs.Add(_log);
    await _context.SaveChangesAsync(cancellationToken);

    _log = null;
  }
}
