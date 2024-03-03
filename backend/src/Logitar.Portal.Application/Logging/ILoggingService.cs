using Logitar.EventSourcing;

namespace Logitar.Portal.Application.Logging;

public interface ILoggingService
{
  void Open(string? correlationId = null, string? method = null, string? destination = null, string? source = null, string? additionalInformation = null, DateTime? startedOn = null);
  void Report(DomainEvent @event);
  void Report(Exception exception);
  void SetActivity(object activity);
  void SetOperation(Operation operation);
  Task CloseAndSaveAsync(int statusCode);
}
