using Logitar.Portal.Contracts.Errors;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Logging;

internal class LogEntity
{
  private readonly List<Error> _errors = new();

  public LogEntity(string? correlationId = null, string? method = null, string? destination = null,
    string? source = null, string? additionalInformation = null, DateTime? startedOn = null)
  {
    CorrelationId = correlationId;
    Method = method;
    Destination = destination;
    Source = source;
    AdditionalInformation = additionalInformation;

    StartedOn = startedOn ?? DateTime.UtcNow;
  }

  private LogEntity()
  {
  }

  public long LogId { get; private set; }
  public Guid Id { get; private set; } = Guid.NewGuid();

  public string? CorrelationId { get; private set; }
  public string? Method { get; private set; }
  public string? Destination { get; private set; }
  public string? Source { get; private set; }
  public string? AdditionalInformation { get; private set; }

  public string? OperationType { get; private set; }
  public string? OperationName { get; private set; }
  public void SetOperation(string type, string name)
  {
    OperationType = type;
    OperationName = name;
  }

  public int? StatusCode { get; private set; }
  public void Complete(int? statusCode = null, DateTime? endedOn = null)
  {
    StatusCode = statusCode;

    EndedOn = endedOn ?? DateTime.UtcNow;
  }

  public DateTime StartedOn { get; private set; }
  public DateTime? EndedOn { get; private set; }
  public TimeSpan? Duration
  {
    get => EndedOn.HasValue ? EndedOn.Value - StartedOn : null;
    private set { }
  }

  public Guid ActorId { get; private set; }
  public Guid? UserId { get; private set; }
  public Guid? SessionId { get; private set; }

  public bool IsCompleted
  {
    get => StatusCode.HasValue;
    private set { }
  }
  public string Level
  {
    get => _errors.GetLogLevel().ToString();
    private set { }
  }
  public bool HasErrors => _errors.Any();
  public string? Errors
  {
    get => _errors.Any() ? $"[{string.Join(',', _errors.Select(error => error.Serialize()))}]" : null;
    private set { }
  }
}
