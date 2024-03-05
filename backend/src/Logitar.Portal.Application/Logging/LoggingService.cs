using Logitar.EventSourcing;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Logging;

internal class LoggingService : ILoggingService
{
  private Log? _log = null;

  private readonly ICacheService _cacheService;
  private readonly ILogRepository _logRepository;

  public LoggingService(ICacheService cacheService, ILogRepository logRepository)
  {
    _cacheService = cacheService;
    _logRepository = logRepository;
  }

  public void Open(string? correlationId, string? method, string? destination, string? source, string? additionalInformation, DateTime? startedOn)
  {
    if (_log != null)
    {
      throw new InvalidOperationException($"You must close the current log by calling one of the '{nameof(CloseAndSaveAsync)}' methods before opening a new log.");
    }

    _log = Log.Open(correlationId, method, destination, source, additionalInformation);
  }

  public void Report(DomainEvent @event)
  {
    _log?.Report(@event);
  }

  public void Report(Exception exception)
  {
    _log?.Report(exception);
  }

  public void SetActivity(object activity)
  {
    _log?.SetActivity(activity);
  }

  public void SetOperation(Operation operation)
  {
    _log?.SetOperation(operation);
  }

  public void SetRealm(Realm realm)
  {
    if (_log != null)
    {
      _log.TenantId = realm.GetTenantId();
    }
  }

  public void SetApiKey(ApiKey apiKey)
  {
    if (_log != null)
    {
      _log.ApiKeyId = new ApiKeyId(apiKey.Id);
    }
  }

  public void SetSession(Session session)
  {
    if (_log != null)
    {
      _log.SessionId = new SessionId(session.Id);
    }
  }

  public void SetUser(User user)
  {
    if (_log != null)
    {
      _log.UserId = new UserId(user.Id);
    }
  }

  public async Task CloseAndSaveAsync(int statusCode, CancellationToken cancellationToken)
  {
    if (_log == null)
    {
      throw new InvalidOperationException($"You must open a new log by calling one of the '{nameof(Open)}' methods before calling the current method.");
    }

    _log.Close(statusCode);

    if (ShouldSaveLog())
    {
      await _logRepository.SaveAsync(_log, cancellationToken);
    }

    _log = null;
  }

  private bool ShouldSaveLog()
  {
    ILoggingSettings? loggingSettings = _cacheService.Configuration?.LoggingSettings;
    if (loggingSettings != null && _log != null)
    {
      if (!loggingSettings.OnlyErrors || _log.HasErrors)
      {
        switch (loggingSettings.Extent)
        {
          case LoggingExtent.ActivityOnly:
            return _log.Activity != null;
          case LoggingExtent.Full:
            return true;
        }
      }
    }

    return false;
  }
}
