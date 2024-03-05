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

  public string? ActivityType { get; private set; }
  public string? ActivityData { get; private set; }

  public int? StatusCode { get; private set; }
  public bool IsCompleted { get; private set; }

  public LogLevel Level { get; private set; }
  public bool HasErrors { get; private set; }

  public DateTime StartedOn { get; private set; }
  public DateTime? EndedOn { get; private set; }
  public TimeSpan? Duration { get; private set; }

  public string? TenantId { get; private set; }
  public string ActorId { get; private set; } = EventSourcing.ActorId.DefaultValue;
  public string? ApiKeyId { get; private set; }
  public string? UserId { get; private set; }
  public string? SessionId { get; private set; }

  public List<LogEventEntity> Events { get; private set; } = [];
  public List<LogExceptionEntity> Exceptions { get; private set; } = [];

  public LogEntity(Log log, JsonSerializerOptions? serializerOptions = null)
  {
    UniqueId = log.Id;

    CorrelationId = log.CorrelationId;
    Method = log.Method;
    Destination = log.Destination;
    Source = log.Source;
    AdditionalInformation = log.AdditionalInformation;

    if (log.Operation != null)
    {
      OperationType = log.Operation.Type;
      OperationName = log.Operation.Name;
    }

    if (log.Activity != null)
    {
      Type activityType = log.Activity.GetType();
      ActivityType = activityType.GetNamespaceQualifiedName();
      ActivityData = JsonSerializer.Serialize(log.Activity, activityType, serializerOptions);
    }

    StatusCode = log.StatusCode;
    IsCompleted = log.IsCompleted;

    Level = log.Level;
    HasErrors = log.HasErrors;

    StartedOn = log.StartedOn.ToUniversalTime();
    EndedOn = log.EndedOn?.ToUniversalTime();
    Duration = log.Duration;

    TenantId = log.TenantId?.Value;
    ActorId = log.ActorId.Value;
    ApiKeyId = log.ApiKeyId?.Value;
    UserId = log.UserId?.Value;
    SessionId = log.SessionId?.Value;

    foreach (DomainEvent @event in log.Events)
    {
      Events.Add(new LogEventEntity(this, @event));
    }
    foreach (Exception exception in log.Exceptions)
    {
      Exceptions.Add(new LogExceptionEntity(this, exception, serializerOptions));
    }
  }

  private LogEntity()
  {
  }
}
