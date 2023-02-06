namespace Logitar.Portal.Infrastructure.Entities
{
  internal class LogEntity
  {
    public LogEntity()
    {
      Id = Guid.NewGuid();
    }

    public long LogId { get; private set; }
    public Guid Id { get; private set; }

    public string TraceIdentifier { get; private set; } = string.Empty;
    public string? IpAddress { get; private set; }
    public string? AdditionalInformation { get; private set; }

    public string Method { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;

    public int StatusCode { get; private set; }

    public DateTime StartedOn { get; private set; }
    public DateTime EndedOn { get; private set; }
    public TimeSpan Duration
    {
      get => EndedOn - StartedOn;
      private set { }
    }

    public string ActorId { get; private set; } = string.Empty;
    public string Actor { get; private set; } = string.Empty;
    public string? ApiKeyId { get; private set; }
    public string? UserId { get; private set; }
    public string? SessionId { get; private set; }

    public string? RequestType { get; private set; }
    public string? RequestData { get; private set; }

    public string? Errors { get; private set; }
    public bool HasErrors
    {
      get => Errors != null;
      private set { }
    }
    public string Level { get; private set; } = string.Empty;

    public List<LogEventEntity> Events { get; private set; } = new();

    public void AddEvents(IEnumerable<EventEntity> events)
    {
      foreach (EventEntity @event in events)
      {
        Events.Add(new LogEventEntity(@event, this));
      }
    }
    public void Complete(int statusCode, string actorId, string actor, string? apiKeyId, string? userId, string? sessionId, string? errors, string level)
    {
      StatusCode = statusCode;

      EndedOn = DateTime.UtcNow;

      ActorId = actorId;
      Actor = actor;
      ApiKeyId = apiKeyId;
      UserId = userId;
      SessionId = sessionId;

      Errors = errors;
      Level = level;
    }
    public void SetRequest(string type, string data)
    {
      RequestType = type;
      RequestData = data;
    }
    public void Start(string traceIdentifier, string? ipAddress, string? additionalInformation, string method, string url)
    {
      TraceIdentifier = traceIdentifier;
      IpAddress = ipAddress;
      AdditionalInformation = additionalInformation;

      Method = method;
      Url = url;

      StartedOn = DateTime.UtcNow;
    }
  }
}
