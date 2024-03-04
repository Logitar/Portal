using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Infrastructure;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Logging;

internal class LoggingService : ILoggingService // TODO(fpion): should be in the Infrastructure layer!
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly ICacheService _cacheService;
  private readonly PortalContext _context;

  private LogEntity? _log = null;

  public LoggingService(ICacheService cacheService, PortalContext context, IServiceProvider serviceProvider)
  {
    _cacheService = cacheService;
    _context = context;

    _serializerOptions.Converters.AddRange(serviceProvider.GetLogitarPortalJsonConverters());
  }

  public void Open(string? correlationId, string? method, string? destination, string? source, string? additionalInformation, DateTime? startedOn)
  {
    if (_log != null)
    {
      throw new InvalidOperationException($"You must close the current log by calling one of the '{nameof(CloseAndSaveAsync)}' methods before opening a new log.");
    }

    _log = new(correlationId, method, destination, source, additionalInformation);
  }

  public void Report(DomainEvent @event)
  {
    AssertLogIsOpen();
    _log!.Report(@event);
  }

  public void Report(Exception exception)
  {
    AssertLogIsOpen();
    _log!.Report(exception, _serializerOptions);
  }

  public void SetActivity(object activity)
  {
    AssertLogIsOpen();
    _log!.SetActivity(activity, _serializerOptions);
  }

  public void SetOperation(Operation operation)
  {
    AssertLogIsOpen();
    _log!.SetOperation(operation);
  }

  public void SetRealm(Realm realm)
  {
    AssertLogIsOpen();
    _log!.TenantId = realm.GetTenantId().Value;
  }

  public void SetApiKey(ApiKey apiKey)
  {
    AssertLogIsOpen();
    _log!.ApiKeyId = new AggregateId(apiKey.Id).Value;
  }

  public void SetSession(Session session)
  {
    AssertLogIsOpen();
    _log!.SessionId = new AggregateId(session.Id).Value;
  }

  public void SetUser(User user)
  {
    AssertLogIsOpen();
    _log!.UserId = new AggregateId(user.Id).Value;
  }

  public async Task CloseAndSaveAsync(int statusCode, CancellationToken cancellationToken)
  {
    AssertLogIsOpen();
    _log!.Close(statusCode);

    if (ShouldSaveLog())
    {
      _context.Logs.Add(_log);
      await _context.SaveChangesAsync(cancellationToken);
    }

    _log = null;
  }

  private void AssertLogIsOpen()
  {
    if (_log == null)
    {
      throw new InvalidOperationException($"You must open a new log by calling one of the '{nameof(Open)}' methods before calling the current method.");
    }
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
            return _log.ActivityType != null;
          case LoggingExtent.Full:
            return true;
        }
      }
    }

    return false;
  }
}
