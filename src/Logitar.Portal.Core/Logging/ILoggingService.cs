namespace Logitar.Portal.Core.Logging;

public interface ILoggingService
{
  Task StartAsync(string? correlationId = null, string? method = null, string? destination = null,
    string? source = null, string? additionalInformation = null, DateTime? startedOn = null,
    CancellationToken cancellationToken = default);
  Task SetActorsAsync(Guid actorId, Guid? userId = null, Guid? sessionId = null, CancellationToken cancellationToken = default);
  Task SetOperationAsync(string type, string name, CancellationToken cancellationToken = default);
  Task EndAsync(int? statusCode = null, DateTime? endedOn = null, CancellationToken cancellationToken = default);
}
