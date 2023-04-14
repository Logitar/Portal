using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Core.Caching;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Core.Logging;
using Logitar.Portal.Core.Realms;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Logging;

internal class LoggingService : ILoggingService
{
  private LogEntity? _log;

  private readonly ICacheService _cacheService;
  private readonly EventContext _eventContext;
  private readonly PortalContext _portalContext;

  public LoggingService(ICacheService cacheService, EventContext eventContext, PortalContext portalContext)
  {
    _cacheService = cacheService;
    _eventContext = eventContext;
    _portalContext = portalContext;
  }

  public Task StartAsync(string? correlationId, string? method, string? destination,
    string? source, string? additionalInformation, DateTime? startedOn, CancellationToken cancellationToken)
  {
    if (_log != null)
    {
      throw new InvalidOperationException($"You must end the current log by calling one of the '{nameof(EndAsync)}' methods before starting a new log.");
    }

    _log = new(correlationId, method, destination, source, additionalInformation, startedOn);

    return Task.CompletedTask;
  }

  public async Task<Guid> StartActivityAsync(object activity, CancellationToken cancellationToken)
    => await StartActivityAsync(activity, startedOn: null, cancellationToken);
  public Task<Guid> StartActivityAsync(object activity, DateTime? startedOn, CancellationToken cancellationToken)
  {
    AssertLogHasBeenStarted();

    return Task.FromResult(_log!.StartActivity(activity, startedOn));
  }

  public async Task AddErrorAsync(Error error, CancellationToken cancellationToken)
    => await AddErrorAsync(error, activityId: null, cancellationToken);
  public Task AddErrorAsync(Error error, Guid? activityId, CancellationToken cancellationToken)
  {
    AssertLogHasBeenStarted();

    _log!.AddError(error, activityId);

    return Task.CompletedTask;
  }

  public async Task AddEventAsync(DomainEvent change, CancellationToken cancellationToken)
    => await AddEventAsync(change, activityId: null, cancellationToken);
  public Task AddEventAsync(DomainEvent change, Guid? activityId, CancellationToken cancellationToken)
  {
    AssertLogHasBeenStarted();

    _log!.AddEvent(change, activityId);

    return Task.CompletedTask;
  }

  public Task SetActorsAsync(Guid actorId, Guid? userId, Guid? sessionId, CancellationToken cancellationToken)
  {
    AssertLogHasBeenStarted();

    _log!.SetActors(actorId, userId, sessionId);

    return Task.CompletedTask;
  }

  public Task SetOperationAsync(string type, string name, CancellationToken cancellationToken)
  {
    AssertLogHasBeenStarted();

    _log!.SetOperation(type, name);

    return Task.CompletedTask;
  }

  public async Task EndActivityAsync(Guid id, CancellationToken cancellationToken)
    => await EndActivityAsync(id, endedOn: null, cancellationToken);
  public Task EndActivityAsync(Guid id, DateTime? endedOn, CancellationToken cancellationToken)
  {
    AssertLogHasBeenStarted();

    _log!.EndActivity(id, endedOn);

    return Task.CompletedTask;
  }

  public async Task EndAsync(int? statusCode, DateTime? endedOn, CancellationToken cancellationToken)
  {
    AssertLogHasBeenStarted();

    _log!.Complete(statusCode, endedOn);

    RealmAggregate? realm = _cacheService.PortalRealm;
    if (realm?.CustomAttributes.TryGetValue(nameof(LoggingSettings), out string? json) == true)
    {
      LoggingSettings settings = LoggingSettings.Deserialize(json);
      if ((settings.Extent == LoggingExtent.Full || (settings.Extent == LoggingExtent.ActivityOnly && _log.Activities.Any()))
        && (!settings.OnlyErrors || _log.HasErrors))
      {
        IReadOnlyDictionary<Guid, ActivityEntity?> pendingEvents = _log.PendingEvents;
        Dictionary<Guid, EventEntity> eventIds = await _eventContext.Events.AsNoTracking()
          .Where(e => pendingEvents.Keys.Contains(e.Id))
          .ToDictionaryAsync(e => e.Id, e => e, cancellationToken);
        foreach (var (id, activity) in pendingEvents)
        {
          if (eventIds.TryGetValue(id, out EventEntity? @event))
          {
            _log.AddEvent(@event, activity);
          }
        }

        _portalContext.Logs.Add(_log);
        await _portalContext.SaveChangesAsync(cancellationToken);
      }
    }

    _log = null;
  }

  private void AssertLogHasBeenStarted()
  {
    if (_log == null)
    {
      throw new InvalidOperationException($"You must start a log by calling one of the '{nameof(StartAsync)}' methods before calling the current method.");
    }
  }
}
