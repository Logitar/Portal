using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Infrastructure
{
  internal class LoggingContext : ILoggingContext
  {
    private readonly IInternalLoggingContext _log;

    public LoggingContext(IInternalLoggingContext log)
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

    /// <summary>
    /// TODO(fpion): refactor
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="statusCode"></param>
    /// <param name="actor"></param>
    /// <param name="apiKey"></param>
    /// <param name="user"></param>
    /// <param name="session"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task CompleteAsync(Configuration configuration, int statusCode, ActorModel actor, ApiKeyModel? apiKey, UserModel? user, SessionModel? session, CancellationToken cancellationToken)
    {
      await _log.CompleteAsync(configuration, statusCode, actor, apiKey, user, session, cancellationToken);
    }
  }
}
