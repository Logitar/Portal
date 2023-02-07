using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Infrastructure
{
  internal class LoggingService : ILoggingService
  {
    private readonly ILoggingContext _log;

    public LoggingService(ILoggingContext log)
    {
      _log = log;
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
      await _log.CompleteAsync(statusCode, actor, apiKey, user, session, cancellationToken);
    }
  }
}
