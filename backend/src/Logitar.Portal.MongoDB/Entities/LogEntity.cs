using Logitar.EventSourcing;
using Logitar.Portal.Application.Logging;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Logitar.Portal.MongoDB.Entities;

internal class LogEntity
{
  [BsonId]
  public ObjectId LogId { get; private set; }

  [BsonRepresentation(BsonType.String)]
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

  [JsonConverter(typeof(JsonStringEnumConverter))]
  [BsonRepresentation(BsonType.String)]
  public LogLevel Level { get; private set; }
  public bool HasErrors { get; private set; }

  public DateTime StartedOn { get; private set; }
  public DateTime? EndedOn { get; private set; }
  public TimeSpan? Duration { get; private set; }

  [BsonRepresentation(BsonType.String)]
  public Guid? TenantId { get; private set; }
  [BsonRepresentation(BsonType.String)]
  public Guid? ActorId { get; private set; }
  [BsonRepresentation(BsonType.String)]
  public Guid? ApiKeyId { get; private set; }
  [BsonRepresentation(BsonType.String)]
  public Guid? UserId { get; private set; }
  [BsonRepresentation(BsonType.String)]
  public Guid? SessionId { get; private set; }

  [BsonRepresentation(BsonType.String)]
  public List<Guid> EventIds { get; private set; } = [];

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

    TenantId = log.TenantId?.ToGuid();
    ActorId = log.ActorId?.ToGuid();
    ApiKeyId = log.ApiKeyId;
    UserId = log.UserId;
    SessionId = log.SessionId;

    foreach (DomainEvent @event in log.Events)
    {
      EventIds.Add(@event.Id.ToGuid());
    }
    foreach (Exception exception in log.Exceptions)
    {
      Exceptions.Add(new LogExceptionEntity(exception, serializerOptions));
    }
  }

  private LogEntity()
  {
  }
}
