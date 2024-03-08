using Logitar.EventSourcing;
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
  void SetRealm(Realm? realm); // TODO(fpion): call when scoping an activity into a Realm
  void SetApiKey(ApiKey? apiKey);
  void SetSession(Session? session);
  void SetUser(User? user); // TODO(fpion): call when impersonating an User
  Task CloseAndSaveAsync(int statusCode, CancellationToken cancellationToken = default);
}
