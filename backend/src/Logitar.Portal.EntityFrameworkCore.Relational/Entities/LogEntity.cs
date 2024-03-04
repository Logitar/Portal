using Logitar.EventSourcing;
using Logitar.Portal.Application.Logging;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal class LogEntity
{
  public long LogId { get; private set; }
  public Guid UniqueId { get; private set; }

  public string? CorrelationId { get; private set; }
  public string? Method { get; private set; }
  public string? Destination { get; private set; }
  public string? Source { get; private set; }
  public string? AdditionalInformation { get; private set; }

  public string? OperationType { get; private set; }
  public string? OperationName { get; private set; }
  public void SetOperation(Operation operation)
  {
    OperationType = operation.Type;
    OperationName = operation.Name;
  }

  public string? ActivityType { get; private set; }
  public string? ActivityData { get; private set; }
  public void SetActivity(object activity, JsonSerializerOptions? serializerOptions = null)
  {
    Type activityType = activity.GetType();

    ActivityType = activityType.GetNamespaceQualifiedName();
    ActivityData = JsonSerializer.Serialize(activity, activityType, serializerOptions);
  }

  public int? StatusCode { get; private set; }
  public bool IsCompleted
  {
    get => StatusCode.HasValue;
    private set { }
  }
  public void Close(int statusCode, DateTime? endedOn = null)
  {
    StatusCode = statusCode;

    EndedOn = (endedOn ?? DateTime.Now).ToUniversalTime();
  }

  public DateTime StartedOn { get; private set; }
  public DateTime? EndedOn { get; private set; }
  public TimeSpan? Duration
  {
    get => EndedOn.HasValue ? EndedOn.Value - StartedOn : null;
    private set { }
  }

  // TODO(fpion): Actors

  public List<LogEventEntity> Events { get; private set; } = [];
  public void Report(DomainEvent @event)
  {
    Events.Add(new LogEventEntity(this, @event));
  }

  // TODO(fpion): Level, HasErrors, Errors(Serialized)

  public LogEntity(string? correlationId, string? method, string? destination, string? source, string? additionalInformation, Guid? uniqueId = null, DateTime? startedOn = null)
  {
    UniqueId = uniqueId ?? Guid.NewGuid();

    CorrelationId = correlationId;
    Method = method;
    Destination = destination;
    Source = source;
    AdditionalInformation = additionalInformation;

    StartedOn = (startedOn ?? DateTime.Now).ToUniversalTime();
  }

  private LogEntity()
  {
  }
}
