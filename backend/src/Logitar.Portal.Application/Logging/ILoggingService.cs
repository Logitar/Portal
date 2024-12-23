using Logitar.EventSourcing;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Logging;

public interface ILoggingService
{
  void Open(string? correlationId = null, string? method = null, string? destination = null, string? source = null, string? additionalInformation = null, DateTime? startedOn = null);
  void Report(DomainEvent @event);
  void Report(Exception exception);
  void SetActivity(IActivity activity);
  void SetOperation(Operation operation);
  void SetRealm(RealmModel? realm);
  void SetApiKey(ApiKeyModel? apiKey);
  void SetSession(SessionModel? session);
  void SetUser(User? user);
  Task CloseAndSaveAsync(int statusCode, CancellationToken cancellationToken = default);
}
