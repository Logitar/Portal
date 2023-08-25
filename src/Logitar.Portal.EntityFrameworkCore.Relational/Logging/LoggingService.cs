using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Logging;

internal class LoggingService : ILoggingService
{
  private readonly ICacheService _cacheService;
  private readonly PortalContext _context;
  private readonly DbSet<EventEntity> _events;

  private readonly HashSet<Guid> _eventIds = new();
  private LogEntity? _log;

  public LoggingService(ICacheService cacheService, EventContext eventContext, PortalContext portalContext)
  {
    _cacheService = cacheService;
    _events = eventContext.Events;
    _context = portalContext;
  }

  public void Start(string? correlationId, string? method, string? destination, string? source, string? additionalInformation, DateTime? startedOn)
  {
    if (_log != null)
    {
      throw new InvalidOperationException($"You must end the current log by calling one of the '{nameof(EndAsync)}' methods before starting a new log.");
    }

    _log = new(correlationId, method, destination, source, additionalInformation, startedOn);
  }

  public void AddEvent(DomainEvent @event)
  {
    AssertLogHasBeenStarted();

    _eventIds.Add(@event.Id);
  }

  public void AddException(Exception exception)
  {
    AssertLogHasBeenStarted();

    _log!.Errors.Add(new ExceptionError(exception));
  }

  public void SetActivity(object activity)
  {
    AssertLogHasBeenStarted();

    _log!.SetActivity(activity);
  }

  public void SetActors(Guid actorId, Guid? userId, Guid? sessionId)
  {
    AssertLogHasBeenStarted();

    _log!.SetActors(actorId, userId, sessionId);
  }

  public void SetOperation(Operation operation)
  {
    AssertLogHasBeenStarted();

    _log!.SetOperation(operation);
  }

  public async Task EndAsync(int? statusCode, DateTime? endedOn, CancellationToken cancellationToken)
  {
    AssertLogHasBeenStarted();

    _log!.Complete(statusCode, endedOn);

    ConfigurationAggregate? configuration = _cacheService.Configuration;
    if (configuration != null && _log.ShouldBeSaved(configuration.LoggingSettings))
    {
      EventEntity[] events = await _events.AsNoTracking()
        .Where(e => _eventIds.Contains(e.Id))
        .ToArrayAsync(cancellationToken);
      _log.AddEvents(events);

      _context.Logs.Add(_log);
      await _context.SaveChangesAsync(cancellationToken);
    }

    _eventIds.Clear();
    _log = null;
  }

  private void AssertLogHasBeenStarted()
  {
    if (_log == null)
    {
      throw new InvalidOperationException($"You must start a log by calling one of the '{nameof(Start)}' methods before calling the current method.");
    }
  }
}
