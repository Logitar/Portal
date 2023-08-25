using Logitar.EventSourcing;

namespace Logitar.Portal.Application.Logging;

public interface ILoggingService
{
  void Start(string? correlationId = null, string? method = null, string? destination = null,
    string? source = null, string? additionalInformation = null, DateTime? startedOn = null);
  void AddEvent(DomainEvent @event);
  void AddException(Exception exception);
  void SetActivity(object activity);
  void SetActors(Guid actorId, Guid? userId = null, Guid? sessionId = null);
  void SetOperation(Operation operation);
  Task EndAsync(int? statusCode = null, DateTime? endedOn = null, CancellationToken cancellationToken = default);
}
