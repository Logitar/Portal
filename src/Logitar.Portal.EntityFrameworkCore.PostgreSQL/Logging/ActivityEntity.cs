using Logitar.EventSourcing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Logging;

internal class ActivityEntity
{
  private static readonly JsonSerializerOptions _options = new();
  static ActivityEntity() => _options.Converters.Add(new JsonStringEnumConverter());

  public ActivityEntity(LogEntity log, object data)
  {
    Log = log;
    LogId = log.LogId;

    Type type = data.GetType();
    Type = type.GetName();
    Data = JsonSerializer.Serialize(data, type);
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
}
