using Logitar.Portal.Application;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Logitar.Portal.Infrastructure
{
  internal class LoggingContext : ILoggingContext
  {
    private readonly List<Error> _errors = new();
    private readonly LogEntity _log = new();

    private readonly ICacheService _cacheService;
    private readonly PortalContext _context;
    private readonly IRequestSerializer _requestSerializer;

    public LoggingContext(ICacheService cacheService,
      PortalContext context,
      IRequestSerializer requestSerializer)
    {
      _cacheService = cacheService;
      _context = context;
      _requestSerializer = requestSerializer;
    }

    public void AddError(Exception exception) => AddError(new Error(exception));
    public void AddError(Error error)
    {
      EnsureLogHasStarted();

      _errors.Add(error);
    }

    public void AddEvents(IEnumerable<EventEntity> events)
    {
      EnsureLogHasStarted();

      _log.AddEvents(events);
    }

    public async Task CompleteAsync(int statusCode, ActorModel actor, ApiKeyModel? apiKey, UserModel? user, SessionModel? session, CancellationToken cancellationToken)
    {
      EnsureLogHasStarted();

      string? errors = null;
      LogLevel level = LogLevel.Information;
      if (_errors.Any())
      {
        errors = JsonSerializer.Serialize(_errors);

        HashSet<ErrorSeverity> levels = _errors.Select(e => e.Severity).ToHashSet();
        if (levels.Contains(ErrorSeverity.Critical))
        {
          level = LogLevel.Critical;
        }
        else if (levels.Contains(ErrorSeverity.Failure))
        {
          level = LogLevel.Error;
        }
        else if (levels.Contains(ErrorSeverity.Warning))
        {
          level = LogLevel.Warning;
        }
      }
      _log.Complete(statusCode, actor.Id, new Actor(actor).Serialize(), apiKey?.Id, user?.Id, session?.Id, errors, level.ToString());

      Configuration? configuration = _cacheService.Configuration;
      if (configuration == null
        || configuration.LoggingSettings.Extent == LoggingExtent.None
        || (configuration.LoggingSettings.Extent == LoggingExtent.ActivityOnly && (_log.ActivityType == null || _log.ActivityData == null))
        || (configuration.LoggingSettings.OnlyErrors && !_log.HasErrors))
      {
        return;
      }

      _context.Logs.Add(_log);
      await _context.SaveChangesAsync(cancellationToken);
    }

    public void SetRequest<T>(IRequest<T> request)
    {
      EnsureLogHasStarted();

      string requestData = _requestSerializer.Serialize(request);

      _log.SetActivity(request.GetType().GetName(), requestData);
    }

    public void Start(string traceIdentifier, string method, string url, string? ipAddress, string? additionalInformation)
    {
      if (_log.StartedOn != default)
      {
        throw new InvalidOperationException("The log has already been started.");
      }

      _log.Start(traceIdentifier, ipAddress, additionalInformation, method, url);
    }

    private void EnsureLogHasStarted()
    {
      if (_log.StartedOn == default)
      {
        throw new InvalidOperationException($"The log has not been started. You must call the {nameof(Start)} method before calling the current method.");
      }
    }
  }
}
