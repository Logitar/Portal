using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Core.Caching;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Core.Logging;
using Logitar.Portal.Core.Realms;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Logging;

internal class LoggingService : ILoggingService
{
  private LogEntity? _log = null;

  private readonly ICacheService _cacheService;
  private readonly PortalContext _context;

  public LoggingService(ICacheService cacheService, PortalContext context)
  {
    _cacheService = cacheService;
    _context = context;
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
        _context.Logs.Add(_log);
        await _context.SaveChangesAsync(cancellationToken);
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
