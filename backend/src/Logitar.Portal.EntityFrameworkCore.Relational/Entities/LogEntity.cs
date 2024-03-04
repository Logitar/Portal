using Logitar.EventSourcing;
using Logitar.Portal.Application.Logging;
using Microsoft.Extensions.Logging;

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

  public LogLevel Level
  {
    get => HasErrors ? LogLevel.Error : LogLevel.Information;
    private set { }
  }
  public bool HasErrors
  {
    get => Exceptions.Count > 0;
    private set { }
  }

  public DateTime StartedOn { get; private set; }
  public DateTime? EndedOn { get; private set; }
  public TimeSpan? Duration
  {
    get => EndedOn.HasValue ? EndedOn.Value - StartedOn : null;
    private set { }
  }

  public string? TenantId { get; set; }
  public string ActorId
  {
    get => UserId ?? ApiKeyId ?? EventSourcing.ActorId.DefaultValue;
    private set { }
  }
  public string? ApiKeyId { get; set; }
  public string? UserId { get; set; }
  public string? SessionId { get; set; }

  public List<LogEventEntity> Events { get; private set; } = [];
  public void Report(DomainEvent @event)
  {
    Events.Add(new LogEventEntity(this, @event));
  }

  public List<LogExceptionEntity> Exceptions { get; private set; } = [];
  public void Report(Exception exception, JsonSerializerOptions? serializerOptions = null)
  {
    Exceptions.Add(new LogExceptionEntity(this, exception, serializerOptions));
  }

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
