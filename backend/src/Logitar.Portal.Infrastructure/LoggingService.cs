using Logitar.Portal.Application;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Infrastructure
{
  internal class LoggingService : ILoggingService
  {
    private readonly ILoggingContext _log;
    private readonly IRepository _repository;

    public LoggingService(ILoggingContext log, IRepository repository)
    {
      _log = log;
      _repository = repository;
    }

    public void Start(string traceIdentifier, string method, string url, string? ipAddress, string? additionalInformation)
    {
      _log.Start(traceIdentifier, method, url, ipAddress, additionalInformation);
    }

    public void AddError(Exception exception)
    {
      _log.AddError(exception);
    }

    public async Task CompleteAsync(int statusCode, ActorModel actor, ApiKeyModel? apiKey, UserModel? user, SessionModel? session, CancellationToken cancellationToken)
    {
      Configuration? configuration = await _repository.LoadConfigurationAsync(cancellationToken);
      if (configuration != null)
      {
        await _log.CompleteAsync(configuration, statusCode, actor, apiKey, user, session, cancellationToken);
      }
    }
  }
}
