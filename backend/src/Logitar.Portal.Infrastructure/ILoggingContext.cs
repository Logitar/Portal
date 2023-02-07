using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Infrastructure
{
  public interface ILoggingContext
  {
    void Start(string traceIdentifier, string method, string url, string? ipAddress = null, string? additionalInformation = null);
    void AddError(Exception exception);
    Task CompleteAsync(Configuration configuration, int statusCode, ActorModel actor, ApiKeyModel? apiKey = null, UserModel? user = null, SessionModel? session = null, CancellationToken cancellationToken = default);
  }
}
