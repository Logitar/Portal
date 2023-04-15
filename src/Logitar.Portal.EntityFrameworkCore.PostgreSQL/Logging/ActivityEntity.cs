using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Core.Logging;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Logging;

internal class ActivityEntity
{
  private readonly List<Error> _errors = new();

  public ActivityEntity(LogEntity log, Activity activity, DateTime? startedOn = null)
  {
    Log = log;
    LogId = log.LogId;

    Type = activity.Type.GetName();
    Data = activity.Data;

    StartedOn = startedOn ?? DateTime.UtcNow;
  }

  private ActivityEntity()
  {
  }

  public long ActivityId { get; private set; }
  public Guid Id { get; private set; } = Guid.NewGuid();

  public LogEntity? Log { get; private set; }
  public long LogId { get; private set; }

  public string Type { get; private set; } = string.Empty;
  public string Data { get; private set; } = string.Empty;

  public DateTime StartedOn { get; private set; }
  public DateTime? EndedOn { get; private set; }
  public TimeSpan? Duration
  {
    get => EndedOn.HasValue ? EndedOn.Value - StartedOn : null;
    private set { }
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
  public bool HasErrors
  {
    get => _errors.Any();
    private set { }
  }
  public string? Errors
  {
    get => _errors.Any() ? $"[{string.Join(',', _errors.Select(error => error.Serialize()))}]" : null;
    private set { }
  }
  public void AddError(Error error) => _errors.Add(error);
  public void Complete(DateTime? endedOn = null) => EndedOn = endedOn ?? DateTime.UtcNow;

  public List<LogEventEntity> Events { get; private set; } = new();
}
