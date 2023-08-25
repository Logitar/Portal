using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Contracts.Configurations;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record LogEntity
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  public LogEntity(string? correlationId = null, string? method = null, string? destination = null,
    string? source = null, string? additionalInformation = null, DateTime? startedOn = null)
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());

    Id = Guid.NewGuid();

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
  public Guid Id { get; private set; }

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
  public void SetActivity(object activity)
  {
    Type type = activity.GetType();

    ActivityType = type.GetName();
    ActivityData = JsonSerializer.Serialize(activity, type, _serializerOptions);
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
  public void SetActors(Guid actorId, Guid? userId = null, Guid? sessionId = null)
  {
    ActorId = actorId;
    UserId = userId;
    SessionId = sessionId;
  }

  public bool IsCompleted
  {
    get => EndedOn.HasValue;
    private set { }
  }
  public LogLevel Level
  {
    get
    {
      if (Errors.Any())
      {
        ErrorSeverity severity = Errors.Max(x => x.Severity);
        switch (severity)
        {
          case ErrorSeverity.Critical:
            return LogLevel.Critical;
          case ErrorSeverity.Failure:
            return LogLevel.Error;
          case ErrorSeverity.Warning:
            return LogLevel.Warning;
        }
      }

      return LogLevel.Information;
    }
    private set { }
  }
  public bool HasErrors
  {
    get => Errors.Any();
    private set { }
  }
  public List<Error> Errors { get; private set; } = new();
  public string? ErrorsSerialized
  {
    get => Errors.Any() ? $"[{string.Join(',', Errors.Select(error => error.Serialize()))}]" : null;
    private set { }
  }

  public List<LogEventEntity> Events { get; private set; } = new();
  public void AddEvent(EventEntity @event)
  {
    Events.Add(new LogEventEntity(this, @event));
  }
  public void AddEvents(IEnumerable<EventEntity> events)
  {
    foreach (EventEntity @event in events)
    {
      AddEvent(@event);
    }
  }

  public bool ShouldBeSaved(ILoggingSettings loggingSettings)
  {
    return (loggingSettings.Extent == LoggingExtent.Full || loggingSettings.Extent == LoggingExtent.ActivityOnly && ActivityType != null)
      && (!loggingSettings.OnlyErrors || HasErrors);
  }
}
