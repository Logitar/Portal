using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Portal.Contracts.Errors;
using System.Collections.Concurrent;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Logging;

internal class LogEntity
{
  private readonly ConcurrentDictionary<Guid, ActivityEntity> _activities = new();
  private readonly List<Error> _errors = new();
  private readonly ConcurrentDictionary<Guid, ActivityEntity?> _events = new();

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
  public void AddError(Error error, Guid? activityId = null)
  {
    _errors.Add(error);

    if (activityId.HasValue && _activities.TryGetValue(activityId.Value, out ActivityEntity? activity))
    {
      activity.AddError(error);
    }
  }

  public List<ActivityEntity> Activities { get; private set; } = new();
  public Guid StartActivity(object data, DateTime? startedOn = null)
  {
    ActivityEntity activity = new(this, data, startedOn);
    _ = _activities.TryAdd(activity.Id, activity);

    return activity.Id;
  }
  public void EndActivity(Guid id, DateTime? endedOn = null)
  {
    if (_activities.Remove(id, out ActivityEntity? activity))
    {
      activity.Complete(endedOn);
      Activities.Add(activity);
    }
  }

  public List<LogEventEntity> Events { get; private set; } = new();
  public IReadOnlyDictionary<Guid, ActivityEntity?> PendingEvents => _events.AsReadOnly();
  public void AddEvent(DomainEvent change, Guid? activityId = null)
  {
    ActivityEntity? activity = activityId.HasValue ? _activities[activityId.Value] : null;
    _events[change.Id] = activity;
  }
  public void AddEvent(EventEntity @event, ActivityEntity? activity = null)
  {
    LogEventEntity logEvent = new(@event, this, activity);
    Events.Add(logEvent);
    activity?.Events.Add(logEvent);
  }
}
